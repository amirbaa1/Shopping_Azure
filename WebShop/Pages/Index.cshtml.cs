using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebShop.Model.Basket.DTO;
using WebShop.Model.Products.DTO;
using WebShop.Service.Basket;
using WebShop.Service.Product;

namespace WebShop.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public List<ProductDto> ProductDto { get; set; }
    public ProductDto ProductSearch { get; set; }
    private readonly IProductService _productService;
    private readonly IBasketService _basketService;
    private string UserId = "1";
    public IndexModel(ILogger<IndexModel> logger, IProductService productService, IBasketService basketService)
    {
        _logger = logger;
        _productService = productService;
        _basketService = basketService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        ProductDto = await _productService.GetAllProduct();
        return Page();
    }

    public async Task<IActionResult> OnPostById(Guid id)
    {
        ProductSearch = await _productService.GetProductById(id);
        _logger.LogInformation($"ProductSearch : ---->{JsonConvert.SerializeObject(ProductSearch)}");
        return Page();
    }
    public async Task<IActionResult> OnPostToBasket(Guid productId)
    {
        var product = _productService.GetProductById(productId);
        var basket = _basketService.GetBasketByUserId(UserId);

        var itme = new AddToBasketDto()
        {
            BasketId = basket.Result.id,
            ImageUrl = product.Result.Image,
            ProductId = product.Result.Id,
            ProductName = product.Result.Name,
            Quantity = 1,
            UnitPrice = product.Result.Price,
        };

        await _basketService.AddToBasket(itme, UserId);
        return RedirectToPage("basket/item");
    }
}