namespace WepApi.DTOs.ecommerce.Response;

public class OrderItemResponse
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string? PhoneNameSnapshot { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; } 
    public OrderResponse? OrderResponse { get; set; }
    public ProductResponse? ProductResponse { get; set; }
}