namespace PaymentService.Domain.Model.Dto;

public class OrderDto
{
    public Guid Id { get; set; }
    public int Amount { get; set; }
    public List<PaymentDto> Payments { get; set; }
}