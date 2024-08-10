using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Enums;
using Order.API.Models.Context;
using Order.API.VM;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(configurator => { configurator.UsingRabbitMq((context, _configure) => { _configure.Host(builder.Configuration["RabbitMQ"]); }); });
builder.Services.AddDbContext<OrderAPIDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("NPSQL")));
var app = builder.Build();

app.MapPost("/create-order",async (CreateOrderVM model,OrderAPIDbContext _context) => {

    Order.API.Models.Order order = new()
    {
        BuyerId = Guid.TryParse(model.BuyerId, out Guid _buyerId) ? _buyerId : throw new Exception("Guid Hatasý"),
        OrderItems = model.OrderItems.Select(oi => new Order.API.Models.OrderItem() { Count = oi.Count, Price = oi.Price, ProductId = Guid.Parse(oi.ProductId) }).ToList(),
        OrderStatus = OrderStatus.Suspend,
        TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Count),
    };
    await _context.Orders.AddAsync(order);
    await _context.SaveChangesAsync();
});



app.UseSwagger();
app.UseSwaggerUI();
app.Run();
