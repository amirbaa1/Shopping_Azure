using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.MessageBus;
using OrderService.MessageBus.ReceivedMessage;
using OrderService.MessageBus.ReceivedMessage.Payment;
using OrderService.MessageBus.ReceivedMessage.UpdateProduct;
using OrderService.Model;
using OrderService.Repository.Mail;
using OrderService.Repository.Order;
using OrderService.Repository.Product;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderContext>(x =>
    x.UseNpgsql(builder.Configuration["ConnectionStrings:OrderConnectionString"]), ServiceLifetime.Singleton);

builder.Services.AddTransient<IOrderService, OrderService.Repository.Order.OrderService>();
builder.Services.AddTransient<IOrderdbContext, OrderdbContext>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IRegisterOrderService, RegisterOrderService>();
builder.Services.AddHostedService<ReceivedPaymentOfOrderService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<EmailSetting>();


builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddHostedService<ReceivedOrderMessage>();
builder.Services.AddHostedService<ReceivedUpdateProduct>();

builder.Services.AddTransient<IMessageBus, MessageBus>();


// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(op =>
//     {
//         op.Authority = "http://localhost:63936";
//         op.Audience = "orderservice";
//         op.RequireHttpsMetadata = false; // Disable HTTPS requirement for development
//     });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(op =>
    {
        op.Authority = "http://localhost:5093";
        op.Audience = "orderservice";
        op.RequireHttpsMetadata = false; // Disable HTTPS requirement for development
    });

builder.Services.AddAuthorization(op =>
{
    op.AddPolicy("ManagementOrder", policy =>
        policy.RequireClaim("scope", "orderService.Management"));
});

builder.Services.AddAuthorization(op =>
{
    op.AddPolicy("GetOrder", policy =>
        policy.RequireClaim("scope", "orderService.getorders"));
});


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


//-------------Health----------------//
builder.Services.AddHealthChecks()
    .AddIdentityServer(new Uri("http://localhost:63936"), name: "Identity Server")
    .AddNpgSql(builder.Configuration["ConnectionStrings:OrderConnectionString"])
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

app.Run();