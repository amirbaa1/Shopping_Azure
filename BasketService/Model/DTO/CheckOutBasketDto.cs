namespace BasketService.Model.DTO
{
    public class  CheckOutBasketDto
    {
        public Guid BasketId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
