using ClientServiceEureka.Controllers;
using Steeltoe.Discovery.Client;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;
using Steeltoe.Management.Info;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//-----------------------------------------------//
builder.Services.AddDiscoveryClient(builder.Configuration);

builder.Services.AddSingleton<IInfoContributor, MyInfoContributor>();

builder.Services.AddInfoActuator(builder.Configuration);

builder.Services.AddHealthActuator(builder.Configuration);
//-----------------------------------------------//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(end =>
{
    end.Map<InfoEndpoint>();
    end.Map<HealthEndpoint>();
});
app.MapControllers();

app.Run();