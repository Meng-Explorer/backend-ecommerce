using System.ComponentModel.DataAnnotations;

namespace WepApi.DTOs.ecommerce.Request;

public class CartRequest
{
    [Required]
    public string? UserId { get; set; }
    
}