using PaymentService.Domain.Model.Dto;

namespace PaymentService.Domain.Repository;

public interface IPaymentService
{
    PaymentDto GetPaymentByOrderId(Guid orderId);
    PaymentDto GetPayment(Guid paymentId);
    bool CreatePayment(Guid orderId, int amount);
    void PayDone(Guid paymentId, string author, long refId);
}