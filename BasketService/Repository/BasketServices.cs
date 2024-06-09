using AutoMapper;
using BasketService.Data;
using BasketService.MessageBus;
using BasketService.Model;
using BasketService.Model.DTO;
using BasketService.Model.DTO.Discount;
using BasketService.Model.MessageDto;
using BasketService.Repository.Discount;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BasketService.Repositroy
{
    public class BasketServices : IBasketService
    {
        private readonly BasketdbContext _context;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;
        private readonly string QueryName;
        private readonly ILogger<BasketServices> _logger;
        public BasketServices(BasketdbContext context, IMapper mapper, IMessageBus messageBus,
            IOptions<RabbitMqConfig> _options, ILogger<BasketServices> logger)
        {
            _context = context;
            _mapper = mapper;
            _messageBus = messageBus;
            QueryName = _options.Value.QueueName_BasketCheckout;
            _logger = logger;
        }

        public void AddItemToBasket(AddItemToBasketDto item)
        {
            var basket = _context.Baskets.FirstOrDefault(p => p.Id == item.basketId);

            if (basket == null)
                throw new Exception("Basket not found....!");

            var basketItem = _mapper.Map<BasketItem>(item);
            var productDto = _mapper.Map<ProductDto>(item);

            CreateProduct(productDto);
            basket.Items.Add(basketItem);
            _context.SaveChanges();
        }

        public BasketDto GetBasket(string UserId)
        {
            var basket = _context.Baskets
                .Include(p => p.Items).ThenInclude(p => p.Product)
                .SingleOrDefault(p => p.UserId == UserId);

            if (basket == null)
            {
                return null;
            }

            return new BasketDto
            {
                Id = basket.Id,
                UserId = basket.UserId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Id = item.Id,
                    ProductName = item.Product.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.UnitPrice,
                    ImageUrl = item.Product.ImageUrl,

                }).ToList(),
            };
        }

        public BasketDto GetOrCreateBasketForUser(string userId)
        {
            var basket = _context.Baskets
                .Include(p => p.Items).ThenInclude(p => p.Product)
                .SingleOrDefault(p => p.UserId == userId);
            if (basket == null)
            {
                return CreateBasketForUser(userId);
            }

            return new BasketDto
            {
                Id = basket.Id,
                UserId = basket.UserId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Id = item.Id,
                    ProductName = item.Product.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.UnitPrice,
                    ImageUrl = item.Product.ImageUrl,
                }).ToList(),
            };
        }

        // var basket = _context.Baskets
        //     .Include(p => p.Items)
        //     .SingleOrDefault(p => p.UserId == UserId);
        //
        // if (basket == null)
        // {
        //     return CreateBasketForUser(UserId);
        // }
        //
        // return Task.FromResult(new BasketDto
        // {
        //     Id = basket.Id,
        //     UserId = basket.UserId,
        //     Items = basket.Items.Select(item => new BasketItemDto
        //     {
        //         ProductId = item.ProductId,
        //         Id = item.Id,
        //         ProductName = item.ProductName,
        //         Quantity = item.Quantity,
        //         UnitPrice = item.UnitPrice,
        //         ImageUrl = item.ImageUrl,
        //     }).ToList(),
        // });
        // }

        public Task<string> RemoveItemFromBasket(Guid basketId)
        {
            try
            {
                var item = _context.BasketItems.FirstOrDefault(p => p.BasketId == basketId);
                if (item == null)
                {
                    return Task.FromResult("Not Found Item Id");
                }
                var productId = _context.Products.SingleOrDefault(p => p.ProductId == item.ProductId);
                if (item == null)
                    throw new Exception("BasketItem Not Found...!");
                _context.BasketItems.Remove(item);
                _context.Products.Remove(productId);
                _context.SaveChanges();
                return Task.FromResult("Item removed successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"error : {ex.Message}");
            }
        }

        public void SetQuantities(Guid itemId, int quantity)
        {
            var item = _context.BasketItems.SingleOrDefault(p => p.Id == itemId);
            item.SetQuantity(quantity);
            _context.SaveChanges();
        }

        public void TransferBasket(string anonymousId, string UserId)
        {
            var anonymousBasket = _context.Baskets
                .Include(p => p.Items)
                .ThenInclude(p => p.Product)
                .SingleOrDefault(p => p.UserId == anonymousId);

            if (anonymousBasket == null) return;

            var userBasket = _context.Baskets.SingleOrDefault(p => p.UserId == UserId);
            if (userBasket == null)
            {
                userBasket = new Basket(UserId);
                _context.Baskets.Add(userBasket);
            }

            foreach (var item in anonymousBasket.Items)
            {
                userBasket.Items.Add(new BasketItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                });
            }

            _context.Baskets.Remove(anonymousBasket);

            _context.SaveChanges();
        }

        public async Task ApplyDiscountToBasket(Guid basketId, Guid discountId)
        {
            var basket = await _context.Baskets.FindAsync(basketId);
            if (basket == null)
            {
                throw new Exception("Not Found");
            }

            basket.DiscountId = discountId;
            _context.SaveChangesAsync();
        }

        private BasketDto CreateBasketForUser(string UserId)
        {
            Basket basket = new Basket(UserId);
            _context.Baskets.Add(basket);
            _context.SaveChanges();
            return new BasketDto
            {
                UserId = basket.UserId,
                Id = basket.Id,
            };
        }

        private ProductDto GetProduct(Guid productId)
        {
            var existProduct = _context.Products.SingleOrDefault(p => p.ProductId == productId);
            if (existProduct != null)
            {
                var product = _mapper.Map<ProductDto>(existProduct);
                return product;
            }
            else
            {
                return null;
            }
        }

        private ProductDto CreateProduct(ProductDto product)
        {
            var existProduct = GetProduct(product.ProductId);
            if (existProduct != null)
            {
                return existProduct;
            }
            else
            {
                var newProduct = _mapper.Map<Model.Product>(product);
                _context.Add(newProduct);
                _context.SaveChanges();
                return _mapper.Map<ProductDto>(newProduct);
            }
        }

        public ResultDto CheckOutBasket(CheckOutBasketDto checkOut, IDiscountService discountService)
        {
            // دریافت سبد خرید
            var basket = _context.Baskets.Include(p => p.Items)
                .ThenInclude(p => p.Product)
                .SingleOrDefault(p => p.Id == checkOut.BasketId);

            if (basket == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = $"{nameof(basket)} Not Found!",
                };
            }

            // ارسال پیام برای سرویس Order
            BasketCheckoutMessage message = _mapper.Map<BasketCheckoutMessage>(checkOut);

            int priceTotal = 0;
            Guid CopyBasketID;
            _logger.LogInformation($"Item:--->{basket.Items}");
            foreach (var item in basket.Items)
            {
                var basketItem = new BasketItemMessage
                {
                    BasketItemId = item.BasketId,
                    Name = item.Product.ProductName,
                    ProductId = item.Product.ProductId,
                    Price = item.Product.UnitPrice,
                    Quantity = item.Quantity,
                    Total = item.Product.UnitPrice * item.Quantity
                };
                priceTotal += item.Product.UnitPrice * item.Quantity;
                message.TotalPrice = priceTotal;
                message.BasketItems.Add(basketItem);
            }

            //  دریافت سرویس تخفیف
            DiscountDto discount = null;
            if (basket.DiscountId.HasValue)
            {
                discount = discountService.GetDiscountById(basket.DiscountId.Value);
            }

            if (discount != null)
            {
                message.TotalPrice = priceTotal - discount.Amount;
            }
            else
            {
                message.TotalPrice = priceTotal;
            }

            //ارسال پیام
            _messageBus.SendMessage(message, QueryName);

            //حذف سبد خرید
            //_context.Baskets.Remove(basket);
            //_context.Products.Remove(basket.Items);

            RemoveItemFromBasket(basket.Items[0].BasketId); // itemId
            _context.SaveChanges();
            return new ResultDto
            {
                IsSuccess = true,
                Message = "سفارش با موفقیت ثبت شد",
            };
        }
    }
}