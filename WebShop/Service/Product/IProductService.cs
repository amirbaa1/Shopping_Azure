using WebShop.Model.Products.DTO;

namespace WebShop.Service.Product;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProduct();
    Task<ProductDto> GetProductById(Guid Id);
}