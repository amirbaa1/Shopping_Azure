namespace WebShop.Model.Pay.DTO;

public class PaymentDto
{
    public Guid PaymentId { get; set; }
    public int Amount { get; set; }
    public bool IsPay { get; set; }
    public Guid OrderId { get; set; }
}