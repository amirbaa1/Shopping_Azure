using App.Metrics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Model.DTO;
using OrderService.Repository;
using OrderService.Repository.Order;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize(Policy = "GetOrder")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMetrics _metrics;
    private readonly ILogger<OrderController> _logger;
    public OrderController(IOrderService orderService, IMetrics metrics, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _metrics = metrics;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var getOrder = await _orderService.GetOrderById(id);
        return Ok(getOrder);
    }

    [HttpGet("user/{userid}")]
    public async Task<IActionResult> GetOrderByUserID(string userid)
    {
        _metrics.Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
        {
            Name = "get_order"
        });


        var getOrder = await _orderService.GetOrdersByUserId(userid);
        return Ok(getOrder);
    }

    //[HttpPost]
    //public async Task<IActionResult> PostOrder([FromBody] AddOrderDto addOrderDto)
    //{
    //    _orderService.AddOrder(addOrderDto);
    //    return Ok();
    //}

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var get = await _orderService.GetAll();
        return Ok(get);
    }
}