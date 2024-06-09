using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using WebApp.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IProductManagement, ProductManagement>(x =>
{
    x.BaseAddress = new Uri(builder.Configuration["AddressHttp:ocelot"]!);
});


builder.Services.AddAuthentication(op =>
{
    op.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme).AddOpenIdConnect(
    OpenIdConnectDefaults.AuthenticationScheme,
    op =>
    {
        op.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        op.Authority = "https://localhost:5005";
        op.ClientId = "Web_Admin";
        op.ClientSecret = "secret";
        op.ResponseType = "code";
        op.GetClaimsFromUserInfoEndpoint = true;
        op.SaveTokens = true;
        op.Scope.Add("openid");
        op.Scope.Add("profile");
        op.Scope.Add("apigatewayadmin.fullaccess");
        op.Scope.Add("productservice.admin");
    });

builder.Services.AddHttpContextAccessor();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();