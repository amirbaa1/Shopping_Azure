using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using BasketService.Data;
using BasketService.Mapper;
using BasketService.MessageBus;
using BasketService.MessageBus.ReceivedMessage.ProductMessage;
using BasketService.Repository.Discount;
using BasketService.Repositroy;
using BasketService.Repositroy.Product;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<BasketdbContext>(x =>
    x.UseNpgsql(builder.Configuration["ConnectionStrings:BasketConnectionString"]), ServiceLifetime.Singleton);

builder.Services.AddAutoMapper(typeof(ProfileMapper));

builder.Services.AddTransient<IBasketService, BasketServices>();

builder.Services.AddTransient<IDiscountService, DiscountService>();

builder.Services.AddTransient<IProductService, ProductService>();

builder.Services.AddHostedService<ReceivedProductUpdateMessage>();

builder.Services.AddTransient<IMessageBus, MessageBus>();


// builder.Services.AddOptions<>()


// -------------rabbitmq---------------//
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddOptions<RabbitMqConfig>(builder.Configuration["RabbitMq"]);
//-----------------------------------//


//builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog().UseMetrics(op =>
{
    op.EndpointOptions = endpoint =>
    {
        endpoint.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
        endpoint.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
        endpoint.EnvironmentInfoEndpointEnabled = false;
    };
});
builder.Services.AddMetrics();

ConfigLoggin();

//-------------Health----------------//
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration["ConnectionStrings:BasketConnectionString"])
    .AddRabbitMQ(rabbitConnectionString: "amqp://guest:guest@localhost:5672", tags: new string[] { "rabbitMQ" });
//---------------------------------//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

app.Run();

void ConfigLoggin()
{
    var envroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{envroment}.json", optional: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.WithExceptionDetails()
        .Enrich.FromLogContext()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElastic(config, envroment))
        .Enrich.WithProperty("Environment", envroment)
        .ReadFrom.Configuration(config)
        .CreateLogger();
}

static ElasticsearchSinkOptions ConfigureElastic(IConfiguration configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"logstash-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        //IndexFormat = $"{Assembly.GetEntryAssembly().GetName().Name.ToLower() environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
        NumberOfReplicas = 1,
        NumberOfShards = 2,
    };
}