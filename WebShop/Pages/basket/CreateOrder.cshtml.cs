using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShop.Pages.basket
{
    public class CreateOrderModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostCreateOrder()
        {
            return Page();
        }
    }
}
