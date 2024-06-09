using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebShop.Model.Products.DTO;

namespace WebShop.Service.Product;

public class ProductService : IProductService
{
    private readonly HttpClient _client;
    private readonly ILogger<ProductService> _logger;

    public ProductService(HttpClient client, ILogger<ProductService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<List<ProductDto>> GetAllProduct()
    {
        var urlLink = new Uri(_client.BaseAddress, "/api/Product");
        var response = await _client.GetAsync(urlLink);
        var content = await response.Content.ReadAsStringAsync();

        var jsonObject = JObject.Parse(content);
        var result = jsonObject["result"];

        // _logger.LogInformation($"---> {JsonConvert.SerializeObject(result)}");

        return JsonConvert.DeserializeObject<List<ProductDto>>(result.ToString());
    }

    public async Task<ProductDto> GetProductById(Guid Id)
    {
        var urlLink = new Uri(_client.BaseAddress, $"/api/Product/{Id}");
        var response = await _client.GetAsync(urlLink);
        var content = await response.Content.ReadAsStringAsync();
        var jsonObject = JObject.Parse(content);
        var result = jsonObject["result"];
        _logger.LogInformation($"get service---> {JsonConvert.SerializeObject(result)}");
        return JsonConvert.DeserializeObject<ProductDto>(result.ToString());
    }
}