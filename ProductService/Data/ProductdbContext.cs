using Microsoft.EntityFrameworkCore;
using ProductService.Model;

namespace ProductService.Data
{
    public class ProductdbContext : DbContext
    {
        public ProductdbContext(DbContextOptions<ProductdbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
