using WebShop.Model.Discount.DTO;
using WebShop.Model.DTO;

namespace WebShop.Service.Discount;

public interface IDiscountService
{
    ResultDto<DiscountDto> GetDiscountByCode(string code);
    ResultDto<DiscountDto> GetDiscountById(Guid id);
    ResultDto UseDiscount(Guid discountId);
}