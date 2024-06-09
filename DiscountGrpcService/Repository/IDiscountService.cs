using DiscountGrpcService.Model;
using DiscountGrpcService.Model.DTO;

namespace DiscountGrpcService.Repository;

public interface IDiscountService
{
    Task<DiscountDto> GetDiscountByCode(string code);
    Task<DiscountDto> GetDiscountById(Guid id);
    bool UseDiscount(Guid Id);
    bool AddNewDiscount(string Code, int Amount);
}