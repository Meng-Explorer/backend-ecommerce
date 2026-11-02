namespace WepApi.DTOs.ecommerce.Request;

public class CouponUsageRequest
{
    public int CouponId { get; set; }
    public int OrderId { get; set; }
    public string? UserId { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime UsedAt { get; set; }
    
}