using System;
namespace Shared.OrderEvents
{
	public class OrderFailedEvent
	{
		public int OrderId { get; set; }
		public string Message { get; set; }
	}
}

