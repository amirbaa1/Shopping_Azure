using BasketService.Data;

namespace BasketService.Repositroy.Product;

public class ProductService : IProductService
{
    private readonly BasketdbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(BasketdbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public bool UpdateProduct(Guid productId, string productName, int priceProduct)
    {
        _logger.LogInformation($"---->ID:{productId},Name:{productName}");
        var productGet = _context.Products.Find(productId);
        _logger.LogInformation($"--->{productGet}");
        if (productGet != null)
        {
            productGet.ProductName = productName;
            productGet.UnitPrice = priceProduct;
            
            _context.SaveChanges();
            return true;
        }

        return false;
    }
}