namespace WepApi.DTOs.ecommerce.Request;

public class CouponRequest
{
    public string? Code {get; set;}
    public string? DiscountType { get; set; }
    public decimal DiscountValue {get; set;}
    public decimal MinOrderAmount { get; set; }
    public DateTime ExpiryDate { get; set; }  
    public int UsageLimit { get; set; }
    public bool IsActive { get; set; }
    
}