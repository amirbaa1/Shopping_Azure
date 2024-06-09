namespace WebShop.Model.Products.DTO;

public class ProductDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public int Price { get; set; }
    public ProductCategory productCategory { get; set; }
}