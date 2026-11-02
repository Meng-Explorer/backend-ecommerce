namespace WepApi.DTOs.ecommerce.Response;

public class OrderStatusHistoryResponse
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
    
    public OrderResponse? OrderResponse { get; set; }
}