using BasketService.Model;
using Microsoft.EntityFrameworkCore;

namespace BasketService.Data
{
    public class BasketdbContext : DbContext
    {
        public BasketdbContext(DbContextOptions<BasketdbContext> options) : base(options)
        {
        }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
