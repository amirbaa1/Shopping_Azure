using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService.Data;
using OrderService.Model.DTO.Payment;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OrderService.MessageBus.ReceivedMessage.Payment
{
    public class ReceivedPaymentOfOrderService : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly string _qeueName;
        private readonly OrderContext _orderContext;

        public ReceivedPaymentOfOrderService(IOptions<RabbitMqConfig> _option, OrderContext orderContext)
        {
            _qeueName = _option.Value.QueueName_PaymentDone;

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672/")
            };
            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _qeueName, durable: true, exclusive: false, autoDelete: false,
                arguments: null);
            _orderContext = orderContext;
        }


        private readonly string QeueName;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consomer = new EventingBasicConsumer(_channel);

            consomer.Received += (ch, ea) =>
            {
                var contnet = Encoding.UTF8.GetString(ea.Body.ToArray());

                var messagePayDone = JsonConvert.DeserializeObject<PaymentOrderMessage>(contnet);

                var resultMessage = HandleMessage(messagePayDone);

                if (resultMessage)
                {
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };
            _channel.BasicConsume(queue: _qeueName, autoAck: false, consomer);

            return Task.CompletedTask;
        }

        private bool HandleMessage(PaymentOrderMessage paymentOrderMessage)
        {
            var orderPayment = _orderContext.Orders.SingleOrDefault(x => x.Id == paymentOrderMessage.OrderId);
            orderPayment.PaymentIsDone();
            _orderContext.SaveChanges();
            return true;
        }
    }
}