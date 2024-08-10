using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumer;
using Order.API.Enums;
using Order.API.Models.Context;
using Order.API.VM;
using Shared;
using Shared.Event;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(configurator => {
    configurator.AddConsumer<PaymentCompletedEventConsumer>();
    configurator.AddConsumer<PaymentFailedEventConsumer>();
    configurator.UsingRabbitMq((context, _configure) => { 
        
        _configure.Host(builder.Configuration["RabbitMQ"]);
        _configure.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventQueue,e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
        _configure.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailedEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    }); });
builder.Services.AddDbContext<OrderAPIDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("NPSQL")));
var app = builder.Build();

app.MapPost("/create-order", async (CreateOrderVM model, OrderAPIDbContext _context, IPublishEndpoint _endpoint) =>
{

    Order.API.Models.Order order = new()
    {
        BuyerId = Guid.TryParse(model.BuyerId, out Guid _buyerId) ? _buyerId : throw new Exception("Guid Hatasý"),
        OrderItems = model.OrderItems.Select(oi => new Order.API.Models.OrderItem() { Count = oi.Count, Price = oi.Price, ProductId = Guid.Parse(oi.ProductId) }).ToList(),
        OrderStatus = OrderStatus.Suspend,
        TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Count),
    };
    await _context.Orders.AddAsync(order);
    await _context.SaveChangesAsync();
    OrderCreatedEvent @event = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMessage() { Count = oi.Count, Price = oi.Price, ProductId = oi.ProductId }).ToList()
    };
    await _endpoint.Publish(@event);
});



app.UseSwagger();
app.UseSwaggerUI();
app.Run();
