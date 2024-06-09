using System.Net.Http.Headers;
using System.Text;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApp.Admin.Model.Dto;

namespace WebApp.Admin.Services;

public class ProductManagement : IProductManagement
{
    private readonly ILogger<ProductManagement> _logger;
    private readonly HttpClient _httpClient;
    private string _accessToken = null;
    private readonly IHttpContextAccessor _contextAccessor;

    public ProductManagement(ILogger<ProductManagement> logger, HttpClient httpClient,
        IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _httpClient = httpClient;
        _contextAccessor = contextAccessor;
    }

    public async Task<List<ProductDto>> GetListProduct()
    {
        // var link = new Uri(_httpClient.BaseAddress, "/api/Product");

        var link = new Uri(_httpClient.BaseAddress, "/Product");

        var accessTokenn = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenn);

        _logger.LogInformation($"--->{accessTokenn}");
        var getProduct = await _httpClient.GetAsync(link);

        var content = await getProduct.Content.ReadAsStringAsync();

        // return JsonConvert.DeserializeObject<List<ProductDto>>(content);
        var jsonObject = JObject.Parse(content);
        var result = jsonObject["result"];

        // _logger.LogInformation($"---> {JsonConvert.SerializeObject(result)}");

        return JsonConvert.DeserializeObject<List<ProductDto>>(result.ToString());
    }

    public async Task<ResultDto> UpdateProduct(UpdateProductDto updateProductDto)
    {
        // var link = new Uri(_httpClient.BaseAddress, "/api/ProductAdmin");
        var link = new Uri(_httpClient.BaseAddress, "/ProductAdmin");

        var accessTokenn = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenn);

        var updateProduct = _httpClient.PutAsJsonAsync(link, updateProductDto);
        if (updateProduct != null)
        {
            return new ResultDto(IsSuccess: true, Message: "اپدیت شد");
        }

        // You need to return a ResultDto object here
        return new ResultDto(IsSuccess: false, Message: "notFound");
    }

    public async Task<ResultDto> DeleteProduct(Guid produtId)
    {
        // var link = new Uri(_httpClient.BaseAddress, $"/api/ProductAdmin/{produtId}");

        var link = new Uri(_httpClient.BaseAddress, $"/ProductAdmin/{produtId}");

        var accessTokenn = await _contextAccessor.HttpContext.GetTokenAsync("access_token");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenn);

        var delete = _httpClient.DeleteAsync(link);

        if (delete != null)
        {
            return new ResultDto(IsSuccess: true, Message: "حذف شد");
        }

        return new ResultDto(IsSuccess: false, Message: "notFound");
    }

    private async Task<string> GetAccessToken()
    {
        if (!string.IsNullOrWhiteSpace(_accessToken))
        {
            return _accessToken;
        }

        var client = new HttpClient();
        var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:7158");

        var token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = discoveryDocument.TokenEndpoint,
            ClientId = "Web_Admin",
            ClientSecret = "secret",
            Scope = "productservice.admin"
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