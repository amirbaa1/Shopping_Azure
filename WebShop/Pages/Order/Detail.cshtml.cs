using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrderService.Model;
using WebShop.Model.Order.DTO;
using WebShop.Service.Order;
using WebShop.Service.Payment;

namespace WebShop.Pages.Order;

public class Detail : PageModel
{
    public Detail(IOrderService orderService, IPaymentService paymentService, ILogger<Detail> logger)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _logger = logger;
    }

    [BindProperty] public OrderLineDetailDto Order { get; set; }

    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<Detail> _logger;

    public async Task<IActionResult> OnGet(Guid orderId)
    {
        Order = await _orderService.GetOrderLineByOrderId(orderId);
        // _logger.LogInformation($"{Order}");
        return Page();
    }
}