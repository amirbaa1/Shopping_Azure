namespace OrderService.Model.DTO.Basket;

public class BasketInfoDto
{
    public string BasketId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string UserId { get; set; }
    public int TotalPrice { get; set; }
    public List<BasketItemOrder> BasketItems { get; set; }
    public string MessageId { get; set; }
    public DateTime CreateTime { get; set; }
}