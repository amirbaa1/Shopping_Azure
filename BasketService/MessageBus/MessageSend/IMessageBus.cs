namespace BasketService.MessageBus;

public interface IMessageBus
{
    void SendMessage(BaseMessage message, string query);
}