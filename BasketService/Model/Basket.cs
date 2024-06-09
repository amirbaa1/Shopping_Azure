

namespace BasketService.Model
{
    public class Basket
    {
        public Basket(string userId)
        {
            UserId = userId;
        }

        public Basket()
        {
        }

        public Guid Id { get; set; }
        public string UserId { get; set; }

        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public Guid? DiscountId { get; set; }
        
    }

}
