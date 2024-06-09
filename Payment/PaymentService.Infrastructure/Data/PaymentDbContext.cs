using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Model;

namespace PaymentService.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
    public DbSet<Payment> payments { get; set; }
    public DbSet<Order> orders { get; set; }

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }
}