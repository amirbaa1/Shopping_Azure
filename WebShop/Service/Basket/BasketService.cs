using Newtonsoft.Json;
using System.Text;
using WebShop.Model.Basket.DTO;
using WebShop.Model.DTO;

namespace WebShop.Service.Basket;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BasketService> _logger;

    public BasketService(HttpClient httpClient, ILogger<BasketService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ResultDto> AddToBasket(AddToBasketDto basket, string userId)
    {
        var url = new Uri(_httpClient.BaseAddress, $"/api/Basket?UserId={userId}");
        _logger.LogInformation($"--->{JsonConvert.SerializeObject(basket)}");

        var response = await _httpClient.PostAsJsonAsync(url, basket);
        _logger.LogInformation($"response--->{JsonConvert.SerializeObject(response)}");

        if (response.IsSuccessStatusCode)
        {
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Item added to basket successfully."
            };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ResultDto
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
    }

    public async Task<ResultDto> DeleteBasket(Guid id)
    {
        var url = new Uri(_httpClient.BaseAddress, $"/api/Basket?ItemId={id}");
        var response = await _httpClient.DeleteAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Item delete !",
            };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ResultDto
            {
                IsSuccess = false,
                Message = errorMessage,
            };
        }
    }

    public async Task<BasketDto> GetBasketByUserId(string id)
    {
        var urlLink = new Uri(_httpClient.BaseAddress, $"/api/Basket/{id}");
        var response = await _httpClient.GetAsync(urlLink);
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<BasketDto>(content);
    }

    public async Task<ResultDto> UpdateBasket(Guid basketItemId, int amount)
    {
        var url = new Uri(_httpClient.BaseAddress, $"/api/Basket?basketItemId={basketItemId}&quantity={amount}");

        var requestContent = new StringContent("", Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, requestContent);

        if (response.IsSuccessStatusCode)
        {
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Item delete !",
            };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ResultDto
            {
                IsSuccess = false,
                Message = errorMessage,
            };
        }
    }

    public async Task<ResultDto> ApplyBasketToDiscount(Guid basketId, Guid discountId)
    {
        var url = new Uri(_httpClient.BaseAddress, $"/api/Basket/{basketId}/{discountId}");

        var requestContent = new StringContent("", Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, requestContent);

        if (response.IsSuccessStatusCode)
        {
            return new ResultDto
            {
                IsSuccess = true,
                Message = "",
            };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ResultDto
            {
                IsSuccess = false,
                Message = errorMessage,
            };
        }
    }

    public async Task<ResultDto> CheckOut(CheckOutBasketDto checkOutBasket)
    {
        var url = new Uri(_httpClient.BaseAddress, $"/api/Basket/CheckOutBasket");
        _logger.LogInformation($"--->{JsonConvert.SerializeObject(checkOutBasket)}");
        var response = await _httpClient.PostAsJsonAsync(url, checkOutBasket);
        if (response.IsSuccessStatusCode)
        {
            return new ResultDto
            {
                IsSuccess = true,
                Message = "",
            };
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ResultDto
            {
                IsSuccess = false,
                Message = errorMessage,
            };
        }
    }
}