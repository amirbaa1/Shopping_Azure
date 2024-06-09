namespace PaymentService.Infrastructure.MessageBus;

public class MessagePaymentDto
{
    public Guid OrderId { get; set; }
    public int Amount { get; set; }
}