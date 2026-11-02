using WepApi.DTOs.Response;

namespace WepApi.DTOs.ecommerce.Response;

public class WishlistResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ProductId { get; set; }
    public string? UserId { get; set; }
    
    public ProductResponse? ProductResponse { get; set; }
    public UserResponse? UserResponse { get; set; }
}