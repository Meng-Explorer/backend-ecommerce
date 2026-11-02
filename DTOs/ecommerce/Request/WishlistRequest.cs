using System.ComponentModel.DataAnnotations;

namespace WepApi.DTOs.ecommerce.Request;

public class WishlistRequest
{
    [Required]
    public string? UserId { get; set; }
    [Required]
    public int? ProductId { get; set; }
    
}