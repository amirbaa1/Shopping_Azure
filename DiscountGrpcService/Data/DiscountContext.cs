using DiscountGrpcService.Model;
using MongoDB.Driver;

namespace DiscountGrpcService.Data;

public class DiscountContext : IDiscountContext
{
    public IMongoCollection<DiscountCode> discounts { get; set; }


    private readonly IConfiguration _configuration;

    public DiscountContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("DiscountStoreDatabase:ConnectionString"));
        var database = client.GetDatabase(configuration.GetValue<string>("DiscountStoreDatabase:DatabaseName"));
        discounts = database.GetCollection<DiscountCode>(
            configuration.GetValue<string>("DiscountStoreDatabase:DiscountsCollectionName"));
    }
}