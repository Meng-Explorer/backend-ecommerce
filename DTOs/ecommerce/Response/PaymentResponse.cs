namespace WepApi.DTOs.ecommerce.Response;

public class PaymentResponse
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? Provider { get; set; }
    public string? TransactionRef { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public DateTime PaidAt { get; set; }
    public OrderResponse? OrderResponse { get; set; }
}