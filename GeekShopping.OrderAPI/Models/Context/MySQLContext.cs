using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Models.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }

        public DbSet<OrderDetails> Details { get; set; }
        public DbSet<OrderHeader> Headers { get; set; }

    }
}
