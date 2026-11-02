using WepApi.DTOs.Response;

namespace WepApi.DTOs.ecommerce.Response;

public class CartResponse
{
    public int Id { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public UserResponse? User { get; set; }
}