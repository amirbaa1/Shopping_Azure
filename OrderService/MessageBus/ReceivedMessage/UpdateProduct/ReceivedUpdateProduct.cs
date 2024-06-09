using System.Text;
using Newtonsoft.Json;
using OrderService.Repository.Product;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.MessageBus.ReceivedMessage.UpdateProduct;

public class ReceivedUpdateProduct : BackgroundService
{
    private readonly ILogger<ReceivedUpdateProduct> _logger;
    private readonly IModel _channel;
    private readonly IConnection _connection;

    private readonly string exchengName;
    private readonly string qeueName_GetMessageUpdateProduct;

    private readonly IProductService _productService;

    public ReceivedUpdateProduct(ILogger<ReceivedUpdateProduct> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
        exchengName = "Update-ProductName";
        qeueName_GetMessageUpdateProduct = "Order_GetMessageOnUpdateProductName";
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672/")
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: exchengName, type: ExchangeType.Fanout, durable: true, autoDelete: false,
            arguments: null);

        _channel.QueueDeclare(queue: qeueName_GetMessageUpdateProduct, durable: true, autoDelete: false,
            exclusive: false);

        _channel.QueueBind(queue: qeueName_GetMessageUpdateProduct, exchange: exchengName, "");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());

            var jsonUpdate = JsonConvert.DeserializeObject<UpdateProductNameMessage>(content);

            var resultHandle = HandlerMessage(jsonUpdate);

            if (resultHandle)
            {
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };
        _channel.BasicConsume(qeueName_GetMessageUpdateProduct, false, consumer);

        return Task.CompletedTask;
    }

    private bool HandlerMessage(UpdateProductNameMessage updateProductNameMessage)
    {
        return _productService.UpdateProduct(updateProductNameMessage.ProductId, updateProductNameMessage.Name,
            updateProductNameMessage.Price);
    }

    private class UpdateProductNameMessage
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}