using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace ProductService.Health;

public class DataBaseHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}