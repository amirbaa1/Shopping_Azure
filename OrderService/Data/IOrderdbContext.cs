using MongoDB.Driver;
using OrderService.Model;

namespace OrderService.Data;

public interface IOrderdbContext
{
    IMongoCollection<Order> Orders { get; set; }
    IMongoCollection<Product> products { get; set; }
}