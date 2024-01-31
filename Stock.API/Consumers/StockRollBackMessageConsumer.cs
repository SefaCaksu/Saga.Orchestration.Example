using System;
using MassTransit;
using MongoDB.Driver;
using Shared.Messages;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class StockRollBackMessageConsumer : IConsumer<StockRollBackMessage>
	{
        readonly MongoDBService _mongoDbService;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public StockRollBackMessageConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpointProvider)
        {
            _mongoDbService = mongoDBService;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<StockRollBackMessage> context)
        {
            IMongoCollection<Models.Stock> collection = _mongoDbService.GetCollection<Models.Stock>();

            foreach (var orderItem in context.Message.OrderItems)
            {
               var stock = await (await collection.FindAsync(c => c.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                stock.Count += orderItem.Count;
                await collection.FindOneAndReplaceAsync(c => c.ProductId == orderItem.ProductId, stock);
            }
        }
    }
}

