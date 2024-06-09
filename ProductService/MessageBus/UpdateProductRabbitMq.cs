namespace ProductService.MessageBus;

public class UpdateProductRabbitMq : BaseMessage
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
}