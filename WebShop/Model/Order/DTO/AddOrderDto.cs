namespace WebShop.Model.Order.DTO;

public class AddOrderDto
{
    public string UserId { get; set; }
    public List<AddOrderLineDto> AddOrderLineDtos { get; set; }
}