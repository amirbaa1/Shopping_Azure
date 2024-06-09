using Microsoft.Extensions.Logging;
using PaymentService.Domain.Model;
using PaymentService.Domain.Model.Dto;
using PaymentService.Domain.Repository;
using PaymentService.Infrastructure.Data;

namespace PaymentService.Application.Repository;

public class PaymentRepository : IPaymentService
{
    private readonly ILogger<PaymentRepository> _logger;
    private readonly PaymentDbContext _context;

    public PaymentRepository(ILogger<PaymentRepository> logger, PaymentDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public PaymentDto GetPaymentByOrderId(Guid orderId)
    {
        var payment = _context.payments.SingleOrDefault(x => x.OrderId == orderId);
        if (payment != null)
        {
            return new PaymentDto
            {
                Amount = payment.Amount,
                OrderId = payment.OrderId,
                PaymentId = payment.Id,
                IsPay = payment.IsPay,
            };
        }
        else
        {
            return null;
        }
    }

    public PaymentDto GetPayment(Guid paymentId)
    {
        var payment = _context.payments.SingleOrDefault(x => x.Id == paymentId);
        if (payment != null)
        {
            return new PaymentDto
            {
                Amount = payment.Amount,
                OrderId = payment.OrderId,
                PaymentId = payment.Id,
                IsPay = payment.IsPay,
            };
        }
        else
        {
            return null;
        }
    }

    public bool CreatePayment(Guid orderId, int amount)
    {
        var getOrder = GetOrder(orderId, amount);
        var getPayment = _context.payments.SingleOrDefault(x => x.OrderId == getOrder.Id);

        if (getPayment != null)
        {
            return true;
        }
        else
        {
            var paymentNew = new Payment
            {
                Amount = amount,
                Id = Guid.NewGuid(),
                // Authority = "NOP", // ???? 
                IsPay = false,
                orders = getOrder,
            };
            _context.payments.Add(paymentNew);
            _context.SaveChanges();
            return true;
        }
    }

    private Order GetOrder(Guid orderId, int amount)
    {
        var getOrder = _context.orders.SingleOrDefault(x => x.Id == orderId);
        if (getOrder != null)
        {
            if (getOrder.Amount != amount)
            {
                getOrder.Amount = amount;
                _context.SaveChanges();
            }

            return getOrder;
        }
        else
        {
            var orderNew = new Order
            {
                Amount = amount,
                Id = orderId,
            };
            _context.orders.Add(orderNew);
            _context.SaveChanges();
            return orderNew;
        }
    }

    public void PayDone(Guid paymentId, string author, long refId)
    {
        var payment = _context.payments.SingleOrDefault(x => x.Id == paymentId);

        payment.DatePay = DateTime.UtcNow;
        payment.IsPay = true;
        payment.Authority = author;
        payment.RefId = refId;

        _context.SaveChanges();
    }
}