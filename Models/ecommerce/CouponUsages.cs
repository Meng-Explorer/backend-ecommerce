namespace WepApi.Models;

public class CouponUsages
{
    public int Id { get; set; }
    public Coupons? Coupon { get; set; }
    public int CouponId { get; set; }
    
    public AspNetUsers? User { get; set; }
    public string? UserId { get; set; }
    
    public Orders? Order { get; set; }
    public int OrderId { get; set; }
    
    public decimal DiscountAmount { get; set; }
    public DateTime UseAt { get; set; }
}