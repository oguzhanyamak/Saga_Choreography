using MassTransit;
using Shared.Event;
using Stock.API.Services;
using MongoDB.Driver;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer(MongoDBServices _services) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var stocks = _services.GetCollection<Models.Stock>();
            foreach (var orderItem in context.Message.OrderItems) {
                var stock = await (await stocks.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();
                stock.Count += orderItem.Count;
                await stocks.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
            }
        }
    }
}
