
using DiscountGrpcService.Data;
using DiscountGrpcService.Mapper;
using DiscountGrpcService.Repository;
using DiscountGrpcService.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IDiscountContext, DiscountContext>();
builder.Services.AddAutoMapper(typeof(DiscountMapper));

builder.Services.AddGrpcHealthChecks().AddCheck(name:"DiscountGRPC",() => HealthCheckResult.Healthy());

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.MapGrpcService<GreeterService>();
app.MapGrpcService<DiscountServiceGrpc>();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
