using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using OrderService.Data;
using OrderService.Model;
using OrderService.Model.DTO.Basket;
using OrderService.Model.DTO.Product;
using OrderService.Repository.Mail;
using OrderService.Repository.Product;

namespace OrderService.Repository.Order;

public class RegisterOrderService : IRegisterOrderService
{
    private readonly OrderContext _orderContext;
    private readonly IProductService _productService;
    private readonly IEmailService _emailService;

    public RegisterOrderService(OrderContext orderContext, IProductService productService, IEmailService emailService)
    {
        _orderContext = orderContext;
        _productService = productService;
        _emailService = emailService;
    }

    public bool Execute(BasketInfoDto basketDto)
    {
        var orderLine = new List<OrderLine>();
        string body = "";  // Initialize the body variable
        foreach (var basketItem in basketDto.BasketItems)
        {
            var product = _productService.GetProduct(new ProductDto
            {
                ProductId = basketItem.ProductId,
                Name = basketItem.Name,
                Price = basketItem.Price
            });


            orderLine.Add(new OrderLine
            {
                Id = Guid.NewGuid(),
                Product = product,
                Quantity = basketItem.Quantity,
                Total = basketItem.Total,
            });

            body += $"Product: {product}, Quantity: {basketItem.Quantity}, Total: {basketItem.Total}\n";
        }

        var order = new Model.Order(basketDto.UserId,
            basketDto.FirstName,
            basketDto.LastName,
            basketDto.Address,
            basketDto.PhoneNumber,
            basketDto.TotalPrice,
            orderLine);


        order.OrderPlaced = DateTime.UtcNow;

        _orderContext.Orders.Add(order);
        _orderContext.SaveChanges();

        var emailCreate = new Email
        {
            Body = body,
            From = "amir.2002.ba@gmail.com",
            Subject = "Order Shopping",
            To = order.Address
        };

        _emailService.sendEmail(emailCreate);
        return true;
    }
}