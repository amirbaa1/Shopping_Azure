namespace PaymentService.Domain.Model;

public class Payment
{
    public Guid Id { get; set; }
    public int Amount { get; set; }
    public bool IsPay { get; set; }
    public DateTime? DatePay { get; set; }
    public string? Authority { get; set; }
    public long RefId { get; set; } = 0;

    public Guid OrderId { get; set; }
    public Order orders { get; set; }
}