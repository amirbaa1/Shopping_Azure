using BasketService.Model.DTO;
using BasketService.Model.DTO.Discount;

namespace BasketService.Repository.Discount
{
    public interface IDiscountService
    {
        DiscountDto GetDiscountById(Guid discountId);
        ResultDto<DiscountDto> GetDiscountByCode(string code);
    }
}
