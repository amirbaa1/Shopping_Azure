namespace OrderService.MessageBus;

public class RabbitMqConfig
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string QueueName_BasketCheckout { get; set; }
    public string QueueName_OrderSendToPayment { get; set; }
    public string QueueName_PaymentDone { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}