using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Model.DTO;

public class OrderLineDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfDefault]
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public int ProductPrice { get; set; }
    public int Quantity { get; set; }
    public int Total { get; set; }
}
