using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Model;

public class OrderLine
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public int Total { get; set; }
    [ForeignKey("Order")] 
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
}