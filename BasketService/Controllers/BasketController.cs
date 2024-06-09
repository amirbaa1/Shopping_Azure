using App.Metrics;
using BasketService.Model.DTO;
using BasketService.Repository.Discount;
using BasketService.Repositroy;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IMetrics _metrics;
        private readonly ILogger<BasketController> _logger;
        public BasketController(IBasketService basketService, IMetrics metrics, ILogger<BasketController> logger)
        {
            _basketService = basketService;
            _metrics = metrics;
            _logger = logger;
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetBasket(string UserId)
        {

            _metrics.Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
            {
                Name = "get_basket_userId"
            });


            _logger.LogInformation("-------");
            _logger.LogWarning("Log warning");

            var basket = _basketService.GetOrCreateBasketForUser(UserId);
            return Ok(basket);
        }

        [HttpPost]
        public async Task<IActionResult> PostBasket(AddItemToBasketDto request, string UserId)
        {
            var basket = _basketService.GetOrCreateBasketForUser(UserId);
            request.basketId = Guid.Parse(basket.Id.ToString());
            _basketService.AddItemToBasket(request);
            var basketData = _basketService.GetBasket(UserId);
            return Ok(basketData);
        }

        [HttpDelete]
        public IActionResult Remove(Guid itemId)
        {
            var remove = _basketService.RemoveItemFromBasket(itemId);
            return Ok(remove);
        }

        [HttpPut]
        public IActionResult SetQuantity(Guid basketItemId, int quantity)
        {
            _basketService.SetQuantities(basketItemId, quantity);
            return Ok();
        }

        [HttpPut("{basketId}/{discountId}")]
        public async Task<IActionResult> ApplyDiscountToBasket(Guid basketId, Guid discountId)
        {
            await _basketService.ApplyDiscountToBasket(basketId, discountId);
            return Accepted();
        }

        [HttpPost("CheckOutBasket")]
        public async Task<IActionResult> CheckoutBasket(CheckOutBasketDto check, [FromServices] IDiscountService discountService)
        {

            _metrics.Measure.Counter.Increment(new App.Metrics.Counter.CounterOptions
            {
                Name = "send_to_rabbitmq_order"
            });


            var result = _basketService.CheckOutBasket(check, discountService);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}