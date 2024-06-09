using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BasketService.MessageBus;

public class MessageBus : IMessageBus
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _password;
    private readonly string _username;
    private IConnection _connection;
    private readonly ILogger<MessageBus> _logger;

    public MessageBus(IOptions<RabbitMqConfig> options, ILogger<MessageBus> logger)
    {
        _logger = logger;
        _host = options.Value.HostName;
        _port = options.Value.Port;
        _username = options.Value.UserName;
        _password = options.Value.Password;
    }

    public void SendMessage(BaseMessage message, string query)
    {
        if (CheckRabbitMQConnection())
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: query,
                    durable: true, exclusive: false, autoDelete: false,
                    arguments: null);
                var json = JsonConvert.SerializeObject(message);
                _logger.LogInformation($"message for send rabbitmq--->{json}");
                var body = Encoding.UTF8.GetBytes(json);
                var Properties = channel.CreateBasicProperties();
                Properties.Persistent = true;

                channel.BasicPublish(exchange: "", routingKey: query
                    , basicProperties: Properties, body: body);
            }
        }
    }

    private void CreateRabbitMQConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                // HostName = "amqp://guest:guest@localhost:5672/",
                // Port = _port,
                // UserName = _username,
                // Password = _password
                
                Uri = new Uri("amqp://guest:guest@localhost:5672/")
            };
            _connection = factory.CreateConnection();
            _logger.LogInformation($"--->{factory}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"can not create connection: {ex.Message}");
        }
    }

    private bool CheckRabbitMQConnection()
    {
        if (_connection != null)
        {
            return true;
        }

        CreateRabbitMQConnection();
        return _connection != null;
    }
}