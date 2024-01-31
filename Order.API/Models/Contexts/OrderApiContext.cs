using Microsoft.EntityFrameworkCore;

namespace Order.API.Models.Contexts
{
    public class OrderApiContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public OrderApiContext(DbContextOptions options) : base(options)
        {
        }
    }
}

