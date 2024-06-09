using DiscountGrpcService.Model;
using DiscountGrpcService.Model.DTO;
using MongoDB.Driver;

namespace DiscountGrpcService.Data;

public interface IDiscountContext
{
    IMongoCollection<DiscountCode> discounts { get; set; }
}