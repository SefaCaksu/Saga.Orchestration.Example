using System;
using Order.API.Enums;

namespace Order.API.Models
{
	public class Order
	{
		public int Id { get; set; }
		public int BuyerId { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public DateTime CreateDate { get; set; }
		public decimal TotalPrice { get; set; }

		public ICollection<OrderItem> OrderItems { get; set; }
	}
}

