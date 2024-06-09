using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ProductService.MessageBus.Message;

public class RabbitMqMessageBus : IMessageBus
{
    private IConnection _connection;
    private readonly ILogger<RabbitMqMessageBus> _logger;


    public RabbitMqMessageBus(ILogger<RabbitMqMessageBus> logger)
    {
        _logger = logger;
        CreateRabbitMqConnection();
    }

    public void SandMessage(BaseMessage message, string exchange)
    {
        if (CheckRabbitMq())
        {
            using (var channel = _connection.CreateModel())
            {
                // channel.QueueDeclare(queue: query, durable: true, exclusive: false, autoDelete: false, arguments: null);

                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout, durable: true, autoDelete: false,
                    arguments: null);

                var json = JsonConvert.SerializeObject(message);

                var body = Encoding.UTF8.GetBytes(json);

                var property = channel.CreateBasicProperties();

                property.Persistent = true;

                channel.BasicPublish(exchange: exchange, routingKey: "", basicProperties: property, body: body);
                _logger.LogInformation("--->Send to rabbintMQ");
            }
        }
    }

    private void CreateRabbitMqConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672/")
            };
            _connection = factory.CreateConnection();
            // _connection.CreateModel();
            _logger.LogInformation($"connection factory rabbitMQ--->{factory}");
        }
        catch (Exception e)
        {
            _logger.LogError($"can not create connection: {e.Message}");
        }
    }

    private bool CheckRabbitMq()
    {
        if (_connection != null)
        {
            return true;
        }

        CreateRabbitMqConnection();
        return _connection != null;
    }
}