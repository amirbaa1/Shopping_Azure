using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace OrderService.MessageBus;

public class MessageBus : IMessageBus
{
    // private readonly string _host;
    // private readonly int _port;
    // private readonly string _password;
    // private readonly string _username;
    private IConnection _connection;
    private readonly ILogger<MessageBus> _logger;


    public MessageBus(ILogger<MessageBus> logger)
    {
        // _host = options.Value.HostName;
        // _port = options.Value.Port;
        // _password = options.Value.Password;
        // _username = options.Value.UserName;
        _logger = logger;
        CreateRabbitMqConnection();
    }

    public void SendMessage(BaseMessage message, string query)
    {
        if (CheckRabbitMQConnection())
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: query, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var json = JsonConvert.SerializeObject(message);

                var body = Encoding.UTF8.GetBytes(json);

                var property = channel.CreateBasicProperties();

                property.Persistent = true;

                channel.BasicPublish(exchange: "", routingKey: query, basicProperties: property, body: body);
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

    private bool CheckRabbitMQConnection()
    {
        if (_connection != null)
        {
            return true;
        }

        CreateRabbitMqConnection();
        return _connection != null;
    }
}