using MassTransit;
using Order.API.Models.Context;
using Shared.Event;

namespace Order.API.Consumer
{
    public class PaymentFailedEventConsumer(OrderAPIDbContext _context) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Models.Order order = await _context.Orders.FindAsync(context.Message.OrderId);
            order.OrderStatus = Enums.OrderStatus.Fail;
            await _context.SaveChangesAsync();
        }
    }
}
