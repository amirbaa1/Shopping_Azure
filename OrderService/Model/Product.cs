namespace OrderService.Model
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public List<OrderLine> orderLines { get; set; }
    }
}
