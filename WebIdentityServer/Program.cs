
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebIdentityServer.Data;
using WebIdentityServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<IdentityDbContext>(x =>
    x.UseNpgsql(builder.Configuration["ConnectionStrings:IdentityConnectionString"]));

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    // .AddTestUsers(new List<Duende.IdentityServer.Test.TestUser>()
    // {
    //     new TestUser()
    //     {
    //         IsActive = true,
    //         Username = "Admin",
    //         Password = "1234",
    //         ProviderName = "amir",
    //         SubjectId = "1",
    //     },
    // })
    .AddInMemoryClients(new List<Client>
    {
        new Client
        {
            ClientName = "Web App",
            ClientId = "Web_APP",
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            AllowedScopes = { "orderService-fullAccess" },
        },
        new Client
        {
            ClientName = "Web AppWEB",
            ClientId = "Web_AppWEB",
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.Code,
            RedirectUris =
            {
                "http://localhost:44306/signin-oidc", // address for webApp
                "https://localhost:7086/signin-oidc" // address for webApp
            },
            PostLogoutRedirectUris =
            {
                "http://localhost:44306/signout-callback-oidc", // address for webApp
                "https://localhost:7086/signout-callback-oidc" // address for webApp
            },
            AllowedScopes = { "openid", "profile", "orderService.getorders", "basketService-fullAccess" },

            AllowOfflineAccess = true,
            AccessTokenLifetime = 60,
            RefreshTokenUsage = TokenUsage.ReUse,
            RefreshTokenExpiration = TokenExpiration.Sliding,
        },
        new Client
        {
            ClientName = "Web AppAdmin",
            ClientId = "Web_Admin",
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            RedirectUris =
            {
                "http://localhost:44370/signin-oidc", // address for admin
                "https://localhost:5001/signin-oidc" // address for admin
            },
            PostLogoutRedirectUris =
            {
                "http://localhost:44370/signout-callback-oidc", // address for admin
                "https://localhost:5001/signout-callback-oidc" // address for admin
            },
            AllowedScopes =
            {
                "openid", "profile", "orderService.getorders", "orderService.Management", "apigatewayadmin.fullaccess",
                "productservice.admin"
            },

            AllowOfflineAccess = true,
            AccessTokenLifetime = 60,
            RefreshTokenUsage = TokenUsage.ReUse,
            RefreshTokenExpiration = TokenExpiration.Sliding,
        },
    }).AddInMemoryApiResources(new List<ApiResource>
    {
        new ApiResource("orderservice", "order service api")
        {
            Scopes = { "orderService.Management", "orderService.getorders" },
        },
        new ApiResource("basketservice", "basket service api")
        {
            Scopes = { "basketService-fullAccess" },
        },
        new ApiResource("apigatewayadmin", "Api gateway for admin")
        {
            Scopes = { "apigatewayadmin.fullaccess" },
        },
        new ApiResource("productservice", "prodcut service admin")
        {
            Scopes = { "productservice.admin" }
        }
    })
    .AddInMemoryApiScopes(new List<ApiScope>
    {
        new ApiScope("orderService-fullAccess"),
        new ApiScope("basketService-fullAccess"),
        new ApiScope("orderService.Management"),
        new ApiScope("orderService.getorders"),
        new ApiScope("apigatewayadmin.fullaccess"),
        new ApiScope("productservice.admin")
    })
    .AddInMemoryIdentityResources(new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    })
    .AddAspNetIdentity<IdentityUser>();


var app = builder.Build();

app.MigrateDatabase<IdentityDbContext>();

// Seed the database (only in Development)
if (app.Environment.IsDevelopment())
{
    SeedUserData.Seed(app);
}

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

app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages();
});


app.Run();