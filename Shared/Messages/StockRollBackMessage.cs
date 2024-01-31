using System;
namespace Shared.Messages
{
	public class StockRollBackMessage
	{
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}

