using OrderService.Model.DTO.Basket;

namespace OrderService.Repository.Order;

public interface IRegisterOrderService
{
    bool Execute(BasketInfoDto basketDto);
}