using System.Text;
using AccountService.Data;
using AccountService.Model;
using AccountService.Model.Dto;
using AccountService.Services;
using AccountService.Services.Token;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//-----------------Data----------------//
builder.Services.AddDbContext<AccountDbContext>(x =>
    x.UseNpgsql(builder.Configuration["ConnectionStrings:AccountConnectionString"]));

builder.Services.AddIdentity<AppUser, IdentityRole>(
        op => { op.SignIn.RequireConfirmedEmail = true; })
    .AddEntityFrameworkStores<AccountDbContext>()
    .AddDefaultTokenProviders();
//-------------------------------------//

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<ResponseDto>();
builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("TokenAuthAPI:JWTOption"));

// ----------------- JWT -------------------//
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op =>
{
    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("TokenAuthAPI:JWTOption:Secret")!)),
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["TokenAuthAPI:JWTOption:Issuer"],
        ValidAudience = builder.Configuration["TokenAuthAPI:JWTOption:Audience"],
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GetOrder", policy =>
        policy.RequireAuthenticatedUser()
            .RequireClaim("scope", "orderService.fullAccess"));
});
//--------------------------------------//

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();