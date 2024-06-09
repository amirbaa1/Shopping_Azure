using MongoDB.Driver;
using OrderService.Model;

namespace OrderService.Data;

public class OrderdbContext : IOrderdbContext
{
    private readonly IConfiguration _configuration;

    public OrderdbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("OrderStoreDatabase:ConnectionString"));
        var database = client.GetDatabase(configuration.GetValue<string>("OrderStoreDatabase:DatabaseName"));
        Orders = database.GetCollection<Order>(configuration.GetValue<string>("OrderStoreDatabase:OrdersCollectionName"));
        products = database.GetCollection<Product>(configuration.GetValue<string>("OrderStoreDatabase:ProductCollectionName"));
    }

    public IMongoCollection<Order> Orders { get; set; }
    public IMongoCollection<Product> products { get; set; }
}