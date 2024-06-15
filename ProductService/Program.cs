using System.Text;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductService.Data;
using ProductService.Health;
using ProductService.MessageBus.Message;
using ProductService.Repository;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductdbContext>(x =>
    x.UseNpgsql(builder.Configuration["ConnectionStrings:ProdcutConnectionString"]));

builder.Services.AddTransient<IProductService, ProductServices>();

builder.Services.AddTransient<ICategoryService, CategoryService>();

builder.Services.AddTransient<IMessageBus, RabbitMqMessageBus>();

//------------------------identityServer4---------------//
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(op =>
//     {
//         op.Authority = "http://localhost:63936";
//         op.Audience = "productservice";
//         op.RequireHttpsMetadata = false;
//     });

// builder.Services.AddAuthorization(op =>
// {
//     op.AddPolicy("ProductAdmin",
//         policy => policy.RequireClaim("scope", "productservice.admin"));
// });
//-----------------------------------------------------//


//------------------------Identity Account.API-------------------//
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(op =>
    {
        op.Authority = "https://localhost:7148";
        op.Audience = "webShop_client";
        op.RequireHttpsMetadata = false;
        op.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "webShop_Api",
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("TokenAuthAPI:JWTOption:Secret")!)),
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization(op =>
{
    op.AddPolicy("ProductAdmin",
        policy => policy.RequireClaim("scope", "productService.Management"));
});
//---------------------------------------------------------------//

//-------------Health----------------//
builder.Services.AddHealthChecks()
    // .AddCheck<DataBaseHealthCheck>("npgslq")
    .AddNpgSql(builder.Configuration["ConnectionStrings:ProdcutConnectionString"])
    // .AddIdentityServer(new Uri("http://localhost:63936"), name: "Identity Server")
    .AddIdentityServer(new Uri("http://localhost:7148"), name: "Identity Costume Account.APi");


builder.Services.AddHealthChecksUI(x =>
    {
        x.AddHealthCheckEndpoint("ProductService", "/health");
        x.SetEvaluationTimeInSeconds(10); // update web health
    })
    .AddInMemoryStorage();
//---------------------------------//

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

// builder.Host.UseSerilog((context, loggerConfiguration) =>
// {
//     loggerConfiguration.MinimumLevel.Debug()
//         .Enrich.FromLogContext()
//         .WriteTo.Console()
//         .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://localhost:9200"))
//         {
//             IndexFormat = $"productServiceLog-{DateTime.Now.Year}",
//             AutoRegisterTemplate = true,
//             AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
//             NumberOfReplicas = 2,
//             NumberOfShards = 1,
//             ModifyConnectionSettings = p => p.BasicAuthentication("elastic", "KDFC4A*K8UdmwrfCl_Pt")
//         });
// }).UseMetrics(op =>
// {
//     op.EndpointOptions = endpoint =>
//     {
//         endpoint.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
//         endpoint.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
//         endpoint.EnvironmentInfoEndpointEnabled = false;
//     };
// });


//-------------serilog-------------------------------//
builder.Host.UseSerilog();
//-------------- Metrics ---------------------------//

// var metrics = AppMetrics.CreateDefaultBuilder()
//     .OutputMetrics.AsPrometheusPlainText()
//     .OutputMetrics.AsPrometheusProtobuf()
//     .Build();
//
// builder.Host.UseMetricsWebTracking();
// builder.Services.AddMetrics(metrics);
// builder.Services.AddMetricsTrackingMiddleware();
// builder.Services.AddMetricsEndpoints(options =>
// {
//     options.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
//     options.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
//     options.EnvironmentInfoEndpointEnabled = false;
// });
//
//
// builder.Services.AddMetrics();
// builder.Services.AddMetricsTrackingMiddleware();
// builder.Services.Configure<KestrelServerOptions>(op => { op.AllowSynchronousIO = true; });
//---------------------------------------------//


configLoggin();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.UseMetricsAllMiddleware();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

app.UseHealthChecksUI(config =>
{
    config.UIPath = "/healthui";
    config.ApiPath = "/healthuiapi";
});

app.Run();

void configLoggin()
{
    var envroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{envroment}.json", optional: true)
        .Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
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