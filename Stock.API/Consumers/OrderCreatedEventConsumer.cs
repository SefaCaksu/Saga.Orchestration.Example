using System;
using MassTransit;
using MassTransit.Transports;
using MongoDB.Driver;
using Shared;
using Shared.OrderEvents;
using Shared.StockEvents;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly MongoDBService _mongoDbService;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderCreatedEventConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpointProvider)
        {
            _mongoDbService = mongoDBService;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            IMongoCollection<Models.Stock> collection = _mongoDbService.GetCollection<Models.Stock>();
            var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));

            foreach (var orderItem in context.Message.OrderItems)
            {
                bool result = (await collection.FindAsync(s =>
                     s.ProductId == orderItem.ProductId
                     && (long)s.Count >= orderItem.Count)).Any();
                stockResult.Add(result);
            }

            if (stockResult.TrueForAll(sr => sr.Equals(true)))
            {
                foreach (var orderItem in context.Message.OrderItems)
                {
                    Models.Stock stock = (await collection.FindAsync(c => c.ProductId == orderItem.ProductId)).FirstOrDefault();
                    stock.Count -= orderItem.Count;
                    await collection.FindOneAndReplaceAsync(c => c.ProductId == orderItem.ProductId, stock);
                }

                StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };

                await sendEndPoint.Send(stockReservedEvent);
            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
                {
                    Message = "Stock bulunamadı"
                };

                await sendEndPoint.Send(stockNotReservedEvent);
            }
        }
    }
}

