using System.ComponentModel.DataAnnotations;

namespace WepApi.DTOs.ecommerce.Request;

public class AddressRequest
{
    [Required]
    public string? UserId { get; set; }
    public string? RecipientName { get; set; }
    public string? Phone {get; set;}
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsDefault { get; set; }
    
    
}