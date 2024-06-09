namespace BasketService.MessageBus;

public class RabbitMqConfig
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string QueueName_BasketCheckout { get; set; }
    public string ExchengName_UpdateProduct { get; set; }
    public string QueueName_GetMessageonUpdateProductName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}