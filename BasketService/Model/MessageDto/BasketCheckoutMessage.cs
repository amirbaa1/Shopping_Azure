using BasketService.MessageBus;

namespace BasketService.Model.MessageDto;

public class BasketCheckoutMessage : BaseMessage
{
    public Guid BasketId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string UserId { get; set; }
    public int TotalPrice { get; set; }

    public List<BasketItemMessage> BasketItems { get; set; } = new List<BasketItemMessage>();
}