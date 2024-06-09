namespace BasketService.Repositroy.Product;

public interface IProductService
{
    bool UpdateProduct(Guid productId, string productName,int priceProduct);
}