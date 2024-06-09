using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace ProductService.Health;

public class DataBaseHealthCheck : IHealthCheck
{
    // public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
    //     CancellationToken cancellationToken = new CancellationToken())
    // {
    //     using (var connect =
    //            new NpgsqlConnection("Host=localhost;Port=5432;Database=Productdb;Username=postgres;Password=amir$$1379"))
    //     {
    //         try
    //         {
    //             await connect.OpenAsync(cancellationToken);
    //             var command = connect.CreateCommand();
    //             command.CommandText = "select 1";
    //             await command.ExecuteNonQueryAsync(cancellationToken);
    //         }
    //         catch (Exception e)
    //         {
    //             return new HealthCheckResult(status: context.Registration.FailureStatus, exception: e);
    //         }
    //     }
    //
    //     return HealthCheckResult.Healthy();
    // }
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}