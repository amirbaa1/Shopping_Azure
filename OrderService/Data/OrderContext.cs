using Microsoft.EntityFrameworkCore;
using OrderService.Model;
using OrderService.Model.DTO;

namespace OrderService.Data;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<Product> Products { get; set; }
}