using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Admin.Model.Dto;
using WebApp.Admin.Services;

namespace WebApp.Admin.Pages.Product;

[Authorize]
public class Index : PageModel
{
    private readonly IProductManagement _productManagement;
    [BindProperty] public List<ProductDto> products { get; set; }

    public Index(IProductManagement productManagement)
    {
        _productManagement = productManagement;
    }

    public void OnGet()
    {
        products = _productManagement.GetListProduct().Result;
    }

    public async Task<IActionResult> OnPostUpdate(UpdateProductDto updateProductDto)
    {
        _productManagement.UpdateProduct(updateProductDto);
        return RedirectToPage("/product/index");
    }

    public async Task<IActionResult> OnPostDeleteProduct(Guid productId)
    {
        _productManagement.DeleteProduct(productId);
        return RedirectToPage("/product/index");
    }
}