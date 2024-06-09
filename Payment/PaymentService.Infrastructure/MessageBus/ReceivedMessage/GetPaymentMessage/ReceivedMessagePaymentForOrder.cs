using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PaymentService.Domain.Repository;
using PaymentService.Infrastructure.MessageBus.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentService.Infrastructure.MessageBus.ReceivedMessage.GetPaymentMessage;

public class ReceivedMessagePaymentForOrder : BackgroundService
{
    private IModel _chennel;
    private IConnection _connection;
    private readonly IPaymentService _paymentService;
    private readonly string _qeueName;
    private readonly ILogger<ReceivedMessagePaymentForOrder> _logger;
    public ReceivedMessagePaymentForOrder(IPaymentService paymentService, IOptions<RabbitMqConfig> options, ILogger<ReceivedMessagePaymentForOrder> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
        _qeueName = "OrderSendToPayment";


        //_qeueName = "OrderSendToPayment";

        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672/")
        };

        _connection = factory.CreateConnection();
        _chennel = _connection.CreateModel();
        _chennel.QueueDeclare(queue: _qeueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_chennel);

        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());

            var message = JsonConvert.DeserializeObject<MessagePaymentDto>(content);
            
            _logger.LogInformation($"message : --->{JsonConvert.DeserializeObject<MessagePaymentDto>(content)}");

            var result = HandleMessage(message.OrderId, message.Amount);
            if (result)
            {
                _chennel.BasicAck(ea.DeliveryTag, false);
            }
        };
        _chennel.BasicConsume(_qeueName, false, consumer);

        return Task.CompletedTask;
    }

    private bool HandleMessage(Guid orderId, int amount)
    {
        return _paymentService.CreatePayment(orderId, amount);
    }
}