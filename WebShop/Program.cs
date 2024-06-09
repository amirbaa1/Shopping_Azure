using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Serilog;
using WebShop.Service.Basket;
using WebShop.Service.Discount;
using WebShop.Service.Order;
using WebShop.Service.Payment;
using WebShop.Service.Product;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAccessTokenManagement();

builder.Services.AddHttpClient<IProductService, ProductService>(cli =>
{
    cli.BaseAddress = new Uri(builder.Configuration["AddressHttp:ProductUrl"]!);
});
builder.Services.AddHttpClient<IBasketService, WebShop.Service.Basket.BasketService>(cli =>
{
    cli.BaseAddress = new Uri(builder.Configuration["AddressHttp:BasketUrl"]!);
});

builder.Services.AddHttpClient<IOrderService, WebShop.Service.Order.OrderService>(cli =>
{
    cli.BaseAddress = new Uri(builder.Configuration["AddressHttp:OrderUrl"]!);
}).AddUserAccessTokenHandler();

builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AddressHttp:PayUrl"]!);
});

builder.Services.AddScoped<IDiscountService, DiscountService>();

builder.Services.AddAuthentication(op =>
{
    op.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme).AddOpenIdConnect(
    OpenIdConnectDefaults.AuthenticationScheme,
    op =>
    {
        op.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        op.Authority = "http://localhost:63936";
        op.ClientId = "Web_AppWEB";
        op.RequireHttpsMetadata = false;
        op.ClientSecret = "secret";
        op.ResponseType = "code";
        op.GetClaimsFromUserInfoEndpoint = true;
        op.SaveTokens = true;
        op.Scope.Add("openid");
        op.Scope.Add("profile");
        op.Scope.Add("orderService.getorders");
        op.Scope.Add("basketService-fullAccess");
        op.Scope.Add("offline_access");
    });

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapRazorPages();

//if (app.Environment.IsDevelopment())
//{

//}

app.Run();