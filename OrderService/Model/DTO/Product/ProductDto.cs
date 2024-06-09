namespace OrderService.Model.DTO.Product;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
}