using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Model.DTO;

public class OrderDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfDefault]
    public Guid Id { get; set; }

    public int ItemCount { get; set; }
    public int TotalPrice { get; set; }
    public bool OrderPaid { get; set; }
    public DateTime OrderPlaced { get; set; }
    public List<OrderLineDto> OrderLines { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}