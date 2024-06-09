using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrderService.Model;
using OrderService.Model.Pay.Dto;

namespace WebShop.Model.Order.DTO;

public class OrderDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfDefault]
    public Guid Id { get; set; }

    public string UserId { get; set; }
    public bool OrderPaid { get; set; }
    public DateTime OrderPlaced { get; set; }
    public int itemCount { get; set; }
    public double totalPrice { get; set; }
    public List<OrderLineDto> OrderLines { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}