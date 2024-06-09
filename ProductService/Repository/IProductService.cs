using ProductService.Model;
using ProductService.Model.DTO;

namespace ProductService.Repository
{
    public interface IProductService
    {
        Task<string> AddProduct(AddNewProductDto addNewProductDto);
        Task<List<ProductDto>> GetProductList();
        Task<ProductDto> GetProduct(Guid Id);
        Product UpdateProductName(UpdateProductDto updateProduct);
        bool DeleteProduct(Guid productId);
    }
}