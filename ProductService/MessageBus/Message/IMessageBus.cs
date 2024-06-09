namespace ProductService.MessageBus.Message;

public interface IMessageBus
{
    void SandMessage(BaseMessage message, string exchange);
}