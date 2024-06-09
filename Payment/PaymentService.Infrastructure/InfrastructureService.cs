using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Domain.Repository;
using PaymentService.Infrastructure.Data;
using PaymentService.Infrastructure.MessageBus.Config;
using PaymentService.Infrastructure.MessageBus.ReceivedMessage.GetPaymentMessage;
using PaymentService.Infrastructure.MessageBus.SendMessages;

namespace PaymentService.Infrastructure;

public static class InfrastructureService
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection _service,
        IConfiguration _configuration)
    {
        _service.AddDbContext<PaymentDbContext>(x =>
            x.UseNpgsql(_configuration.GetConnectionString("PaymentConnectionString")), ServiceLifetime.Singleton);

        _service.AddHostedService<ReceivedMessagePaymentForOrder>();
        
        //_configuration.GetSection("RabbitMq");
        //_service.Configure<RabbitMqConfig>(_configuration.GetConnectionString("RabbitMq"));
        //_configuration.GetSection("RabbitMq").Get<RabbitMqConfig>();
        _service.AddOptions<RabbitMqConfig>(_configuration.GetConnectionString("RabbitMq")); // ?

        _service.AddTransient<IMessageBus, RabbitMQMessageBus>();

        return _service;
    }
}