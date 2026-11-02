namespace WepApi.DTOs.ecommerce.Request;

public class OrderStatusHistoryRequest
{
    public int OrderId { get; set; }
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? ChangedBy { get; set; }
}