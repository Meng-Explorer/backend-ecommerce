using System.ComponentModel.DataAnnotations;

namespace WepApi.DTOs.ecommerce.Request;

public class OrderRequest
{
    public string? OrderNumber { get; set; }
    [Required]
    public string? UserId { get; set; }
    [Required]
    public int AddressId { get; set; }
    public string? AddressSnapshot { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status {get; set;}
    public string? PaymentStatus { get; set; }
    
}