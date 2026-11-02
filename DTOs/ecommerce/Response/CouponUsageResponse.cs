using WepApi.DTOs.Response;

namespace WepApi.DTOs.ecommerce.Response;

public class CouponUsageResponse
{
    public int Id { get; set; }
    public int CouponId { get; set; }
    public string? UserId { get; set; }
    public int OrderId { get; set; }

    public decimal DiscountAmount { get; set; }
    public DateTime UseAt { get; set; }
    
    public CouponResponse? CouponResponse { get; set; }
    public UserResponse? UserResponse { get; set; }
    public OrderResponse? OrderResponse { get; set; }
}