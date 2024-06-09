using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebShop.Model.Basket.DTO;
using WebShop.Model.DTO;
using WebShop.Service;
using WebShop.Service.Basket;
using WebShop.Service.Discount;
using WebShop.Service.Product;
using static WebShop.Model.Basket.DTO.BasketDto;


namespace WebShop.Pages.basket
{
    [Authorize]
    public class itemModel : PageModel
    {
        private readonly IBasketService _basketService;
        private readonly string UserId = "1";
        private readonly IProductService _productService;
        private readonly IDiscountService _discountService;
        private readonly ILogger<itemModel> _logger;
        [BindProperty] public BasketDto Basket { get; set; }
        [BindProperty] public CheckOutBasketDto CheckOutBasketDto { get; set; }

        public itemModel(IBasketService basketService, IProductService productService, IDiscountService discountService,
            ILogger<itemModel> logger)
        {
            _basketService = basketService;
            _productService = productService;
            _discountService = discountService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            Basket = await _basketService.GetBasketByUserId(UserId);
            if (Basket.DiscountId.HasValue)
            {
                var discount = _discountService.GetDiscountById(Basket.DiscountId.Value);
                Basket.DiscountDetail = new DiscountInBasketDto
                {
                    Amount = discount.Data.Amount,
                    DiscountCode = discount.Data.Code,
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteBasket(Guid id)
        {
            await _basketService.DeleteBasket(id);
            return RedirectToPage("/basket/item");
        }

        public async Task<IActionResult> OnPostToBasket(Guid productId)
        {
            var product = _productService.GetProductById(productId);
            var basket = _basketService.GetBasketByUserId(UserId);

            var item = new AddToBasketDto()
            {
                BasketId = basket.Id.ToString(),
                ImageUrl = product.Result.Image,
                ProductId = product.Result.Id,
                ProductName = product.Result.Name,
                Quantity = 1,
                UnitPrice = product.Result.Price,
            };

            await _basketService.AddToBasket(item, UserId);
            return RedirectToPage("Index");
        }

        public IActionResult OnPostEditBasket(Guid BasketItemId, int quantity)
        {
            _basketService.UpdateBasket(BasketItemId, quantity);
            return RedirectToPage("basket/item");
        }

        public async Task<IActionResult> OnPostApplyDiscount(string discountCode)
        {
            if (string.IsNullOrWhiteSpace(discountCode))
            {
                return new JsonResult(new ResultDto
                {
                    IsSuccess = true,
                    Message = "لطفا کد تخفیف را وارد کنید"
                });
            }

            var discount = _discountService.GetDiscountByCode(discountCode);
            if (discount.IsSuccess == true)
            {
                if (discount.Data.Used)
                {
                    return new JsonResult(new ResultDto
                    {
                        IsSuccess = false,
                        Message = "این کد قبلا استفاده شده است."
                    });
                }

                var basket = _basketService.GetBasketByUserId(UserId);
                _basketService.ApplyBasketToDiscount(Guid.Parse(basket.Result.id), discount.Data.Id);
                _discountService.UseDiscount(discount.Data.Id);

                return new JsonResult
                (
                    new ResultDto
                    {
                        IsSuccess = true,
                        Message = "این کد با موفقیت فعال شد."
                    }
                );
            }
            else
            {
                return new JsonResult(new ResultDto
                {
                    IsSuccess = false,
                    Message = discount.Message
                });
            }
        }

        public async Task<IActionResult> OnPostBasketCheckout()
        {
            return RedirectToPage("checkout");
        }
    }
}