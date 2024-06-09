namespace OrderService.Model.DTO;

public class AddOrderDto
{
    public string UserId { get; set; }
    public List<AddOrderLineDto> AddOrderLineDtos { get; set; }
}