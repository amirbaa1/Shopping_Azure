

namespace BasketService.Model.Basket
{
    public class Basket
    {
        public Basket(string UserId)
        {
            this.UserId = UserId;
        }
        public Basket()
        {
        }
        public Guid Id { get; set; }
        public string UserId { get; private set; }

        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public double TotalPrice()
        {
            return Items.Sum(x => x.UnitPrice * x.Quantity);
        }
    }

}
