using OrderService.Model.DTO;

namespace OrderService.Repository.Order;

public interface IOrderService
{
    //void AddOrder(AddOrderDto addOrderDto);
    Task<List<OrderDto>> GetOrdersByUserId(string userid);
    Task<OrderLineDetailDto> GetOrderById(Guid id);
    Task<List<OrderDto>> GetAll();
    Task<ResultDto> RequestPayment(Guid orderId);
}