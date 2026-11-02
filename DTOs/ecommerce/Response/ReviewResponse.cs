using WepApi.DTOs.Response;

namespace WepApi.DTOs.ecommerce.Response;

public class ReviewResponse
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ProductId { get; set; }
    public string? UserId { get; set; }
    public UserResponse? UserResponse { get; set; }
    public ProductResponse? ProductResponse { get; set; }
}