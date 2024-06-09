using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService.Model.DTO.Basket;
using OrderService.Repository.Order;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace OrderService.MessageBus.ReceivedMessage;

public class ReceivedOrderMessage : BackgroundService
{
    private IModel _channel;
    private IConnection _connection;
    private readonly string _queueName;
    private readonly IRegisterOrderService _registerOrderService;

    public ReceivedOrderMessage(IOptions<RabbitMqConfig> _options, IRegisterOrderService registerOrderService)
    {
        _registerOrderService = registerOrderService;
        _queueName = _options.Value.QueueName_BasketCheckout;
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672/"),
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, eventArgs) =>
        {
            var body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var basket = JsonConvert.DeserializeObject<BasketInfoDto>(body);

            //ثبت سفارش
            var resultHandler = HandlerMessage(basket);
            if (resultHandler)
            {
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            }
        };
        _channel.BasicConsume(queue: _queueName, false, consumer);

        return Task.CompletedTask;
    }

    private bool HandlerMessage(BasketInfoDto basketInfoDto)
    {
        return _registerOrderService.Execute(basketInfoDto);
    }
}