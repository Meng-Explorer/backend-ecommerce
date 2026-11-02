namespace WepApi.DTOs.ecommerce.Request;

public class PaymentRequest
{
    public int OrderId { get; set; }
    public string? Provider { get; set; }
    public string? TransactionRef { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
}