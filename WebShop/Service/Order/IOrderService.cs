using WebShop.Model.DTO;
using WebShop.Model.Order.DTO;

namespace WebShop.Service.Order;

public interface IOrderService
{
    Task<List<OrderDto>> GetOrderByUserId(string userId);
    Task<OrderLineDetailDto> GetOrderLineByOrderId(Guid orderId);
    Task<ResultDto> RequestPayment(Guid orderId);

}