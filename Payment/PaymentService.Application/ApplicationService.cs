using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Repository;
using PaymentService.Domain.Repository;

namespace PaymentService.Application;

public static class ApplicationService
{
    public static IServiceCollection AddApplicationService(this IServiceCollection service)
    {
        service.AddTransient<IPaymentService, PaymentRepository>();
        return service;
    }
}