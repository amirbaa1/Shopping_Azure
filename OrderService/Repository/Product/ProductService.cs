using OrderService.Data;
using OrderService.Model.DTO.Product;

namespace OrderService.Repository.Product;

public class ProductService : IProductService
{
    private readonly OrderContext _orderContext;

    public ProductService(OrderContext orderContext)
    {
        _orderContext = orderContext;
    }

    public Model.Product GetProduct(ProductDto productDto)
    {
        var existProduct = _orderContext.Products.SingleOrDefault(x => x.ProductId == productDto.ProductId);
        if (existProduct != null)
        {
            return existProduct;
        }

        return CreateNewProduct(productDto);
    }

    public bool UpdateProduct(Guid productId, string productName, int priceProduct)
    {
        var productGet = _orderContext.Products.Find(productId);

        if (productGet != null)
        {
            productGet.ProductName = productName;
            // productGet.ProductPrice = priceProduct; // NO Cheng Price

            _orderContext.SaveChanges();
            return true;
        }

        return false;
    }

    private Model.Product CreateNewProduct(ProductDto productDto)
    {
        var newProduct = new Model.Product()
        {
            ProductId = productDto.ProductId,
            ProductName = productDto.Name,
            ProductPrice = productDto.Price
        };
        _orderContext.Products.Add(newProduct);
        _orderContext.SaveChanges();
        return newProduct;
    }
}