namespace OrderService.MessageBus.Message;

public class SendOrderToPay : BaseMessage
{
    public Guid OrderId { get; set; }
    public int Amount { get; set; }
}