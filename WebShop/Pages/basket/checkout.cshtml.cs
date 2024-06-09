using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShop.Model.Basket.DTO;
using WebShop.Service;
using WebShop.Service.Basket;

namespace WebShop.Pages.basket
{
    public class checkoutModel : PageModel
    {
        [BindProperty] public CheckOutBasketDto checkOut { get; set; }
        private readonly IBasketService _basketService;
        private readonly string UserId = "1";

        public checkoutModel(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCheckOut()
        {
            checkOut.UserId = UserId;

            var iddBasket = _basketService.GetBasketByUserId(UserId);

            checkOut.BasketId = Guid.Parse(iddBasket.Result.id);

            var result = await _basketService.CheckOut(checkOut);
            if (result.IsSuccess)
                return RedirectToPage("CreateOrder");
            else
            {
                //افزودن پیام
                return Page();
            }
        }
    }
}