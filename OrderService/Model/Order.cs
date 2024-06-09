using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Model;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public bool OrderPaid { get; set; }
    public DateTime OrderPlaced { get; set; }
    public List<OrderLine> OrderLines { get; set; }
    public int TotalPrice { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public Order(string userId, string firstName, string lastName, string address, string phoneNumber, int totalPrice,
        List<OrderLine> orderLines)
    {
        UserId = userId;
        OrderPaid = false;
        OrderPlaced = DateTime.UtcNow;
        OrderLines = orderLines;
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        PhoneNumber = phoneNumber;
        TotalPrice = totalPrice;
        PaymentStatus = PaymentStatus.unPaid;
    }

    public Order()
    {
    }

    public void RequestPayment()
    {
        PaymentStatus = PaymentStatus.RequestPayment;
    }

    public void PaymentIsDone()
    {
        OrderPaid = true;
        PaymentStatus = PaymentStatus.isPaid;
    }
}