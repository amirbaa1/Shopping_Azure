using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebShop.Model.Products.DTO;
using WebShop.Service.Product;

namespace WebShop.Pages.Product;

public class Details : PageModel
{
    private readonly ILogger<Details> _logger;
    private readonly IProductService _productService;
    public ProductDto productDto { get; set; }

    public Details(ILogger<Details> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task<IActionResult> OnGet(Guid id)
    {
        productDto = await _productService.GetProductById(id);
        _logger.LogInformation($"---->{JsonConvert.SerializeObject(productDto)}");
        return Page();
    }
}