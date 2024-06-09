using Newtonsoft.Json;
using WebShop.Model.DTO;
using WebShop.Model.Pay.DTO;

namespace WebShop.Service.Payment;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(HttpClient httpClient, ILogger<PaymentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ResultDto<ReturnPaymentLinkDto>> GetPaymentByOrderId(Guid orderId, string callbackUrl)
    {
        var link = new Uri(_httpClient.BaseAddress, $"api/Pay?orderId={orderId}&callbackUrlFront={callbackUrl}");

        var response = await _httpClient.GetAsync(link);

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<ResultDto<ReturnPaymentLinkDto>>(content);
    }
}