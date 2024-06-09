using System.Text;
using Dto.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentService.Domain.Model.Dto;
using PaymentService.Domain.Model.DTO;
using PaymentService.Domain.Repository;
using PaymentService.Infrastructure.MessageBus;
using PaymentService.Infrastructure.MessageBus.SendMessages;
using RestSharp;
using ZarinPal.Class;

namespace Payment.API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class PayController : ControllerBase
{
    private readonly ZarinPal.Class.Payment _paymentZarin;
    private readonly Authority _authority;
    private readonly Transactions _transactions;
    private readonly IPaymentService _paymentService;
    private readonly string merchantId;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PayController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IMessageBus _messageBus;

    public PayController(IPaymentService paymentService, IConfiguration configuration, ILogger<PayController> logger,
        HttpClient httpClient, IMessageBus messageBus)
    {
        _paymentService = paymentService;
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;

        var expose = new Expose();
        _paymentZarin = expose.CreatePayment();
        _authority = expose.CreateAuthority();
        _transactions = expose.CreateTransactions();
        merchantId = configuration["merchantId"]!;
        _messageBus = messageBus;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid orderId, string callbackUrlFront)
    {
        var payGet = _paymentService.GetPaymentByOrderId(orderId);

        if (payGet == null)
        {
            return Ok(new ResultDto
            {
                IsSuccess = false,
                Message = "پرداخت یافت نشد."
            });
        }

        var callBackUrl = Url.Action(nameof(Verify), "Pay",
            new
            {
                paymentId = payGet.PaymentId,
                callbackUrlFront = callbackUrlFront
            }, protocol: Request.Scheme);

        var result = await _paymentZarin.Request(new DtoRequest()
        {
            Amount = payGet.Amount,
            CallbackUrl = callBackUrl,
            Description = "Test",
            Email = "",
            Mobile = "",
            MerchantId = merchantId,
        }, ZarinPal.Class.Payment.Mode.zarinpal);

        // _logger.LogInformation($"result zarinPall--->{JsonConvert.DeserializeObject<DtoRequest>(result.Authority)}");

        _logger.LogInformation($"--->merchantId : {merchantId}");

        string redirectUrl = $"https://sandbox.zarinpal.com/pg/StartPay/{result.Authority}";
        _logger.LogInformation($"Link zarinPall--->{redirectUrl}");

        return Ok(new ResultDto<ReturnPaymentLinkDto>
        {
            IsSuccess = true,
            Data = new ReturnPaymentLinkDto
            {
                PaymentLink = redirectUrl
            }
        });
    }

    [AllowAnonymous]
    [HttpGet("Verify")]
    public async Task<IActionResult> Verify(Guid paymentId, string callBackUrlFront)
    {
        string status = HttpContext.Request.Query["Status"];
        string authority = HttpContext.Request.Query["authority"];

        if (status != "" &&
            status.ToString().ToLower() == "ok" &&
            authority != "")
        {
            var pay = _paymentService.GetPayment(paymentId);
            if (pay == null)
            {
                return NotFound();
            }

            // var client = new RestClient("https://www.zarinpal.com/pg/rest/WebGate/PaymentVerification.json");
            // client.Timeout = -1;
            // var request = new RestRequest(Method.POST);

            string url = "https://sandbox.zarinpal.com/pg/rest/WebGate/PaymentVerification.json";

            var json = JsonConvert.SerializeObject(new
            {
                MerchantId = merchantId,
                Authority = authority,
                Amount = pay.Amount
            });


            // $"{{\"MerchantID\": \"{merchendId}\", \"Authority\": \"{Authority}\", \"Amount\": \"{pay.Amount}\"}}";
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation($"--->{data}");
            
            var response = await _httpClient.PostAsync(url, data);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var verification = JsonConvert.DeserializeObject<VerificationPayResultDto>(responseContent);

                if (verification.Status == 100)
                {
                    _paymentService.PayDone(paymentId, authority, verification.RefId);

                    // ارسال پیغام برای سرویس سفارش

                    var paymentDoneMessage = new PaymentIsDoneMessage
                    {
                        CreateTime = DateTime.UtcNow,
                        MessageId = Guid.NewGuid(),
                        orderId = pay.OrderId,
                    };

                    _messageBus.SendMessage(paymentDoneMessage, qeueName: "PaymentDone");

                    return Redirect(callBackUrlFront);
                }
                else
                {
                    return NotFound(callBackUrlFront);
                }
            }
        }


        return Redirect(callBackUrlFront);
    }
}