using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using BasketService.Repositroy.Product;
using Microsoft.Extensions.Options;

namespace BasketService.MessageBus.ReceivedMessage.ProductMessage
{
    public class ReceivedProductUpdateMessage : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly string exchengName;
        private readonly string qeueName_GetMessageUpdateProduct;

        private readonly ILogger<ReceivedProductUpdateMessage> _logger;
        private readonly IProductService _productService;

        public ReceivedProductUpdateMessage(IProductService productService,
            ILogger<ReceivedProductUpdateMessage> logger)
        {
            try
            {
                _productService = productService;
                _logger = logger;
                // exchengName = _options.Value.ExchengName_UpdateProduct;
                // qeueName_GetMessageUpdateProduct = _options.Value.QueueName_GetMessageonUpdateProductName;

                exchengName = "Update-ProductName";
                qeueName_GetMessageUpdateProduct = "Basket_GetMessageOnUpdateProductName";


                var factory = new ConnectionFactory
                {
                    Uri = new Uri("amqp://guest:guest@localhost:5672/")
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _logger.LogInformation($"----> Connction Factory :{_connection}");

                _channel.ExchangeDeclare(exchange: exchengName, type: ExchangeType.Fanout, durable: true, autoDelete: false,
                    arguments: null);

                _channel.QueueDeclare(queue: qeueName_GetMessageUpdateProduct, durable: true, false, false);

                _channel.QueueBind(queue: qeueName_GetMessageUpdateProduct, exchange: exchengName, routingKey: "");
            }
            catch (Exception ex)
            {
                //throw new Exception($"{ex.Message}");
                _logger.LogError($"Not Connect RabbitMQ, Error :{ex.Message}");
            }
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogError("Channel is not initialized.");
                return null;
            }

            if (string.IsNullOrEmpty(qeueName_GetMessageUpdateProduct))
            {
                _logger.LogError("Queue name is not initialized.");
                throw new Exception("Queue name is not initialized.");
            }

            try
            {
                var consumer = new EventingBasicConsumer(_channel);

                _logger.LogInformation($"---> Consumer : {consumer}");

                consumer.Received += (ch, ea) =>
                {
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var updateJson = JsonConvert.DeserializeObject<UpdateProductNameMessage>(content);

                    _logger.LogInformation($"--->{updateJson}");

                    var resultHandel = HandelMessage(updateJson);

                    _logger.LogInformation($"--->resul handler : {resultHandel}");

                    if (resultHandel)
                    {
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };

                _channel.BasicConsume(qeueName_GetMessageUpdateProduct, false, consumer);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Problem RabbitMQ, Error :{ex.Message}");
                throw;
            }
        }

        private bool HandelMessage(UpdateProductNameMessage updateProductNameMessage)
        {
            return _productService.UpdateProduct(updateProductNameMessage.ProductId,
                updateProductNameMessage.Name, updateProductNameMessage.Price);
        }

        private class UpdateProductNameMessage
        {
            public Guid ProductId { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
        }
    }
}