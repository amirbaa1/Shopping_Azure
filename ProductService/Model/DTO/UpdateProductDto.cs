namespace ProductService.Model.DTO;

public class UpdateProductDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
}