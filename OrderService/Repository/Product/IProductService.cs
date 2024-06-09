using OrderService.Model.DTO.Product;

namespace OrderService.Repository.Product;

public interface IProductService
{
    Model.Product GetProduct(ProductDto productDto);
    bool UpdateProduct(Guid productId, string productName, int priceProduct);
}