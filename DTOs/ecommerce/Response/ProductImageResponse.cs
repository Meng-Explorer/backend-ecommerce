namespace WepApi.DTOs.ecommerce.Response;

public class ProductImageResponse
{
    public int Id { get; set; }
    public int ProductId  { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPrimary { get; set; }
    public ProductResponse? ProductResponse { get; set; }
}