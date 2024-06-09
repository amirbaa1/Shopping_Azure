

using BasketService.Model.DTO;
using BasketService.Repository.Discount;

namespace BasketService.Repositroy
{
    public interface IBasketService
    {
        BasketDto GetOrCreateBasketForUser(string userId);
        BasketDto GetBasket(string UserId);
        void AddItemToBasket(AddItemToBasketDto item);
        Task<string> RemoveItemFromBasket(Guid basketId);
        void SetQuantities(Guid itemId, int quantity);
        void TransferBasket(string anonymousId, string UserId);
        Task ApplyDiscountToBasket(Guid basketId, Guid discountId);
        ResultDto CheckOutBasket(CheckOutBasketDto checkOut, IDiscountService discountService);
    }
}
