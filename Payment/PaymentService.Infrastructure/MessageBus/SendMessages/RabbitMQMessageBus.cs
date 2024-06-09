using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace PaymentService.Infrastructure.MessageBus.SendMessages
{
    public class RabbitMQMessageBus : IMessageBus
    {

        private IConnection _connection;
        private readonly ILogger<RabbitMQMessageBus> _logger;

        public RabbitMQMessageBus(ILogger<RabbitMQMessageBus> logger)
        {
            _logger = logger;
        }

        public void SendMessage(BaseMessage message, string qeueName)
        {
            if (CheckRabbitMQConnection())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(qeueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var json = JsonConvert.SerializeObject(message);

                    var body = Encoding.UTF8.GetBytes(json);

                    var properties = channel.CreateBasicProperties();

                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "", routingKey: qeueName, basicProperties: properties, body: body);
                }
            }
        }

        private void CreateRabbitMQConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
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
}
