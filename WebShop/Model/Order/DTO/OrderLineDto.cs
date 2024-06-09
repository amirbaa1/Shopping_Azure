using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebShop.Model.Order.DTO;

public class OrderLineDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfDefault]
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public double ProductPrice { get; set; }
    public int Quantity { get; set; }
}