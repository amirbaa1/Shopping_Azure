using Microsoft.AspNetCore.Mvc;
using OrderService.Repository.Order;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderPaymentController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderPaymentController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("{orderId}")]
    public async Task<IActionResult> Post(Guid orderId)
    {
        return Ok(await _orderService.RequestPayment(orderId));
    }
}