using System;
using MassTransit;
using Order.API.Models.Contexts;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
	public class OrderFailedEventConsumer : IConsumer<OrderFailedEvent>
	{
        readonly OrderApiContext _contex;

        public OrderFailedEventConsumer(OrderApiContext contex)
        {
            _contex = contex;
        }

        public async Task Consume(ConsumeContext<OrderFailedEvent> context)
        {
            int orderId = context.Message.OrderId;
            var order = await _contex.Orders.FindAsync(orderId);

            if (order != null)
            {
                order.OrderStatus = Enums.OrderStatus.Fail;
                await _contex.SaveChangesAsync();
            }

        }
    }
}

