namespace BasketService.Model.DTO
{
    public class BasketDto
    {
        public BasketDto(string userId)
        {
            UserId = userId;
        }

        public BasketDto()
        {
        }

        public Guid Id { get; set; }
        public string UserId { get; set; }

        public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
        // public int TotalPrice { get; set; }

        public int Total()
        {
            if (Items.Count > 0)
            {
                int total = Items.Sum(p => p.UnitPrice * p.Quantity);
                return total;
            }

            return 0;
        }
    }
}