namespace WepApi.DTOs.ecommerce.Request;

public class OrderItemRequest
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string? PhoneNameSnapshot { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; } 
}