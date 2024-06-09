using System.Net.Http.Headers;
using System.Text;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using WebShop.Model.DTO;
using WebShop.Model.Order.DTO;

namespace WebShop.Service.Order;

public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderService> _logger;
    private string _accessToken = null;
    private readonly IHttpContextAccessor _contextAccessor;
    public OrderService(HttpClient httpClient, ILogger<OrderService> logger, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _contextAccessor = contextAccessor;
    }

    public async Task<List<OrderDto>> GetOrderByUserId(string userId)
    {
        var link = new Uri(_httpClient.BaseAddress, $"/api/order/user/{userId}");

        //await GetAccessToken();
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // var accessTokenn = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
        // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenn);

        // نیاز به استفاده از توکن نیستیم به دلیله از refsher token استفاده شده و AddUserAccessTokenHandler در program.cs
        var response = await _httpClient.GetAsync(link);
        var content = await response.Content.ReadAsStringAsync();
        _logger.LogInformation($"-->{JsonConvert.SerializeObject(content)}");
        return JsonConvert.DeserializeObject<List<OrderDto>>(content);
    }

    public async Task<OrderLineDetailDto> GetOrderLineByOrderId(Guid orderId)
    {
        var link = new Uri(_httpClient.BaseAddress, $"/api/order/{orderId}");

        //var toke = await GetAccessToken();
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", toke);


        var accessTokenn = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenn);


        var response = await _httpClient.GetAsync(link);
        var content = await response.Content.ReadAsStringAsync();
        _logger.LogInformation($"list order ---> {JsonConvert.SerializeObject(content)}");
        return JsonConvert.DeserializeObject<OrderLineDetailDto>(content);
    }

    public async Task<ResultDto> RequestPayment(Guid orderId)
    {
        var link = new Uri(_httpClient.BaseAddress, $"/api/orderpayment/{orderId}");

        var header = new StringContent("", Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(link, header);

        // if (response.StatusCode == HttpStatusCode.OK)
        // {
        //     return new ResultDto
        //     {
        //         IsSuccess = true,
        //     };
        // }
        // else
        // {
        //     return new ResultDto
        //     {
        //         IsSuccess = false,
        //         Message = response.RequestMessage.ToString(),
        //     };
        // }

        var content = await response.Content.ReadAsStringAsync();

        _logger.LogInformation($"list order ---> {JsonConvert.SerializeObject(content)}");

        var json = JsonConvert.DeserializeObject<ResultDto>(content);

        return json;
    }

    private async Task<string> GetAccessToken()
    {
        if (!string.IsNullOrWhiteSpace(_accessToken))
        {
            return _accessToken;
        }

        var client = new HttpClient();
        var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:5005");

        var token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = discoveryDocument.TokenEndpoint,
            ClientId = "Web_APP",
            ClientSecret = "secret",
            Scope = "orderService-fullAccess"
        });
        if (token.IsError)
        {
            throw new Exception(token.Error);
        }

        _logger.LogInformation($"--->{token} // {token.AccessToken}");

        _accessToken = token.AccessToken;
        return _accessToken;
    }
}