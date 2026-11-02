namespace WepApi.DTOs.ecommerce.Request;

public class ProductImageRequest
{
    public int ProductId { get; set; }
    public IFormFile? ImageUrl { get; set; } 
    public bool IsPrimary { get; set; }
}