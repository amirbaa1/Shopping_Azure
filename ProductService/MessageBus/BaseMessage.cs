namespace ProductService.MessageBus;

public class BaseMessage
{
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
}