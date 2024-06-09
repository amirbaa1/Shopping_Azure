using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using OrderService.Data;
using OrderService.Model;
using OrderService.Model.DTO.Basket;
using OrderService.Model.DTO.Product;
using OrderService.Repository.Product;

namespace OrderService.Repository.Order;

public class RegisterOrderService : IRegisterOrderService
{
    private readonly OrderContext _orderContext;
    private readonly IProductService _productService;

    public RegisterOrderService(OrderContext orderContext, IProductService productService)
    {
        _orderContext = orderContext;
        _productService = productService;
    }

    public bool Execute(BasketInfoDto basketDto)
    {
        var orderLine = new List<OrderLine>();
        foreach (var basketItem in basketDto.BasketItems)
        {
            var product = _productService.GetProduct(new ProductDto
            {
                ProductId = basketItem.ProductId,
                Name = basketItem.Name,
                Price = basketItem.Price
            });

            // var product = new Model.Product
            // {
            //     ProductId = basketItem.ProductId,
            //     ProductName = basketItem.Name,
            //     ProductPrice = basketItem.Price
            // };


            orderLine.Add(new OrderLine
            {
                Id = Guid.NewGuid(),
                Product = product,
                Quantity = basketItem.Quantity,
                Total = basketItem.Total,
            });
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
        return true;
    }
}