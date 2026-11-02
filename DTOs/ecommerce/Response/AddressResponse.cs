using WepApi.DTOs.Response;

namespace WepApi.DTOs.ecommerce.Response;

public class AddressResponse
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? RecipientName { get; set; }
    public string? Phone { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsDefault { get; set; }
    public UserResponse? User { get; set; }
}