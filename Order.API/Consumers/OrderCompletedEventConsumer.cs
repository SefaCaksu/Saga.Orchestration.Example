using System;
using MassTransit;
using Order.API.Models.Contexts;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
    {
        readonly OrderApiContext _contex;

        public OrderCompletedEventConsumer(OrderApiContext contex)
        {
            _contex = contex;
        }

        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
            int orderId = context.Message.OrderId;
            var order = await _contex.Orders.FindAsync(orderId);

            if(order != null)
            {
                order.OrderStatus = Enums.OrderStatus.Completed;
                await _contex.SaveChangesAsync();
            }

        }
    }
}

