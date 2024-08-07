namespace WebApp.Admin.Model.Dto;

public class ProductDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public int? Price { get; set; }
}