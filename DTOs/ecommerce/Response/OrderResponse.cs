using WepApi.DTOs.Response;

namespace WepApi.DTOs.ecommerce.Response;

public class OrderResponse
{
    public int Id { get; set; }
    public string? OrderNumber { get; set; }
    public string? UserId { get; set; }
    public int? AddressId { get; set; }
    
    public string? AddressSnapshot { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public UserResponse? UserResponse { get; set; }
    public AddressResponse? AddressResponse { get; set; }
}