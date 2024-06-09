using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderService.Model.Pay.Dto;
using WebShop.Model.Order.DTO;
using WebShop.Service.Order;
using WebShop.Service.Payment;

namespace WebShop.Pages.Order;

[Authorize]
public class Index : PageModel
{
    private readonly string userId = "1";
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<Index> _logger;
    [BindProperty] public List<OrderDto> Orders { get; set; }

    public Index(IOrderService orderService, IPaymentService paymentService, ILogger<Index> logger)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet()
    {
        Orders = await _orderService.GetOrderByUserId(userId);
        return Page();
    }

    public async Task<IActionResult> OnPost(string userid)
    {
        Orders = await _orderService.GetOrderByUserId(userid);
        return Page();
    }

    public async Task<IActionResult> OnPostPay(Guid orderId)
    {
        var order = await _orderService.GetOrderLineByOrderId(orderId);
        if (order.PaymentStatus == PaymentStatus.isPaid)
        {
            RedirectToPage(nameof(Detail), new { id = orderId });
        }

        if (order.PaymentStatus == PaymentStatus.unPaid)
        {
            //ارسال درخواست پرداخت برای سرویس سفارش
            var request = _orderService.RequestPayment(orderId: orderId);
        }

        //دریافت لینک پرداخت از سرویس پرداخت    
        // string callBack = Url.Action(nameof(OnGet), "Order", new { orderId }, protocol: Request.Scheme);
        string callBack2 = Url.Page("/order/Detail", new { orderId });

        _logger.LogInformation($"---> callBack : {callBack2}");

        var linkPay = await _paymentService.GetPaymentByOrderId(orderId, callBack2);
        _logger.LogInformation($"--->link pay : {linkPay}");

        if (linkPay.IsSuccess)
        {
            _logger.LogInformation($"---> link bank pay : {linkPay.Data.PaymentLink}");
            return Redirect(linkPay.Data.PaymentLink);
        }
        else
        {
            return NotFound();
        }
    }
}