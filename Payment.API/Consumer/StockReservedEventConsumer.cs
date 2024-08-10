using MassTransit;
using Shared.Event;

namespace Payment.API.Consumer
{
    public class StockReservedEventConsumer(IPublishEndpoint _endpoint) : IConsumer<StockReservedEvent>
    {
        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            if (true) {
                PaymentCompletedEvent @event = new PaymentCompletedEvent() {  OrderId = context.Message.OrderId };
                await _endpoint.Publish(@event);
            } else {
                PaymentFailedEvent @event = new PaymentFailedEvent() { OrderId = context.Message.OrderId,Message = "Ödeme Sorunu",OrderItems=context.Message.OrderItems };
                await _endpoint.Publish(@event);
            }
        }
    }
}
