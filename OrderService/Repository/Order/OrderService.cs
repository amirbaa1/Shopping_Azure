using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using OrderService.Data;
using OrderService.MessageBus;
using OrderService.MessageBus.Message;
using OrderService.Model;
using OrderService.Model.DTO;

namespace OrderService.Repository.Order;

public class OrderService : IOrderService
{
    private readonly OrderContext _context;
    private readonly ILogger<OrderService> _logger;
    private readonly IOrderdbContext _orderdbContext;
    private readonly IMessageBus _messageBus;
    private readonly string QueueName_Payment;

    public OrderService(OrderContext context, ILogger<OrderService> logger, IOrderdbContext orderdbContext,
        IMessageBus messageBus, IOptions<RabbitMqConfig> options)
    {
        _context = context;
        _logger = logger;
        _orderdbContext = orderdbContext;
        _messageBus = messageBus;
        QueueName_Payment = options.Value.QueueName_OrderSendToPayment;
    }

    public Task<List<OrderDto>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<OrderLineDetailDto> GetOrderById(Guid id)
    {
        var order = _context.Orders.Include(x => x.OrderLines)
            .ThenInclude(x => x.Product)
            .FirstOrDefault(x => x.Id == id);
        if (order == null)
        {
            return null;
        }

        var result = new OrderLineDetailDto
        {
            Id = order.Id,
            Address = order.Address,
            FirstName = order.FirstName,
            LastName = order.LastName,
            PhoneNumber = order.PhoneNumber,
            UserId = order.UserId,
            OrderPide = order.OrderPaid,
            OrderPlaced = order.OrderPlaced,
            PaymentStatus = order.PaymentStatus, // پرداخت .
            OrderLines = order.OrderLines.Select(x => new OrderLineDto
            {
                Id = x.Id,
                ProductName = x.Product.ProductName,
                ProductPrice = x.Product.ProductPrice,
                Quantity = x.Quantity,
                Total = x.Total
            }).ToList(),
            TotalPrice = order.TotalPrice,
        };
        return result;
    }

    public Task<List<OrderDto>> GetOrdersByUserId(string userid)
    {
        var orders = _context.Orders.Include(x => x.OrderLines)
            .Where(x => x.UserId == userid)
            .Select(x => new OrderDto
            {
                Id = x.Id,
                OrderPaid = x.OrderPaid,
                ItemCount = x.OrderLines.Count(),
                TotalPrice = x.TotalPrice,
                OrderPlaced = x.OrderPlaced,
                PaymentStatus = x.PaymentStatus, // پرداخت . 
            }).ToListAsync();
        return orders;
    }

    public async Task<ResultDto> RequestPayment(Guid orderId)
    {
        var order = await _context.Orders.SingleOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
        {
            return new ResultDto
            {
                IsSuccess = false,
                Message = "پرداخت نشد."
            };
        }

        // ارسال پیام پرداخت برای سرویس پرداخت
        var orderToPay = new SendOrderToPay()
        {
            Amount = order.TotalPrice,
            CreateTime = DateTime.UtcNow,
            MessageId = Guid.NewGuid(),
            OrderId = order.Id,
        };

        _messageBus.SendMessage(orderToPay, QueueName_Payment);

        //تغییر وضعیت پرداخت سفارش
        order.RequestPayment();
        _context.SaveChanges();
        return new ResultDto
        {
            IsSuccess = true,
            Message = "درخواست پرداخت ثبت شد."
        };
    }
}