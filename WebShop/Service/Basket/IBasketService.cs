using WebShop.Model.Basket.DTO;
using WebShop.Model.DTO;

namespace WebShop.Service.Basket;

public interface IBasketService
{
    Task<BasketDto> GetBasketByUserId(string id);
    Task<ResultDto> AddToBasket(AddToBasketDto basket, string userId);
    Task<ResultDto> DeleteBasket(Guid id);
    Task<ResultDto> UpdateBasket(Guid basketItemId, int amount);
    Task<ResultDto> ApplyBasketToDiscount(Guid basketId, Guid discountId);
    Task<ResultDto> CheckOut(CheckOutBasketDto checkOutBasket);
}