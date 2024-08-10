using MassTransit;
using Order.API.Models.Context;
using Shared.Event;

namespace Order.API.Consumer
{
    public class PaymentCompletedEventConsumer(OrderAPIDbContext _context) : IConsumer<PaymentCompletedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            Models.Order order = await _context.Orders.FindAsync(context.Message.OrderId);
            if(order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            else
            {
                order.OrderStatus = Enums.OrderStatus.Completed;
                await _context.SaveChangesAsync();
            }
        }
    }
}
