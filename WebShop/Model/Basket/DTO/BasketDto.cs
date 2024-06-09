namespace WebShop.Model.Basket.DTO
{
    public class BasketDto
    {
        public BasketDto(string UserId)
        {
            this.UserId = UserId;
        }

        public BasketDto()
        {
        }

        public string id { get; set; }
        public string UserId { get; set; }
        public Guid? DiscountId { get; set; }
        public DiscountInBasketDto DiscountDetail { get; set; } = null;

        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();

        public double TotalPrice()
        {
            int result = Items.Sum(x => x.UnitPrice * x.Quantity);
            if (DiscountId.HasValue)
            {
                result = result - DiscountDetail.Amount;
            }
            return result;
        }
        public class DiscountInBasketDto
        {
            public int Amount { get; set; }
            public string DiscountCode { get; set; }
        }
    }
}