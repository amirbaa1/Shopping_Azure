using WebShop.Model.DTO;
using WebShop.Model.Pay.DTO;

namespace WebShop.Service.Payment
{
    public interface IPaymentService
    {
        Task<ResultDto<ReturnPaymentLinkDto>> GetPaymentByOrderId(Guid orderId, string callbackUrl);
    }
}