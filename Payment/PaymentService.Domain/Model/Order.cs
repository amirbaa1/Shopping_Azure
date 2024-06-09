namespace PaymentService.Domain.Model;

public class Order
{
    public Guid Id { get; set; }
    public int Amount { get; set; }
    public List<Payment> Payments { get; set; }
}