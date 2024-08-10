using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Event;
using Stock.API.Models;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(MongoDBServices _service, ISendEndpointProvider _provider, IPublishEndpoint _endpoint) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {

            List<bool> result = new();
            IMongoCollection<Models.Stock> collection = _service.GetCollection<Models.Stock>();
            foreach (var item in context.Message.OrderItems)
            {
                result.Add(await (await collection.FindAsync(s => s.ProductId == item.ProductId && s.Count >= item.Count)).AnyAsync());
            }
            if (result.TrueForAll(s => s.Equals(true)))
            {
                foreach (var item in context.Message.OrderItems)
                {
                    Models.Stock stock = await (await collection.FindAsync(s => s.ProductId == item.ProductId)).FirstOrDefaultAsync();
                    stock.Count -= item.Count;
                    await collection.FindOneAndReplaceAsync(x => x.ProductId == item.ProductId, stock);
                }

                var sendEndpont = await _provider.GetSendEndpoint(new($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));
                StockReservedEvent @event = new() { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId, TotalPrice = context.Message.TotalPrice, OrderItems = context.Message.OrderItems };
                await sendEndpont.Send(@event);

            }
            else
            {
                StockNotReservedEvent @event = new() { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId, Message = "Stock Yetersiz" };
                await _endpoint.Publish(@event);
            }
        }
    }
}
