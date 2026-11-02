using System.ComponentModel.DataAnnotations;

namespace WepApi.DTOs.ecommerce.Request;

public class ReviewRequest
{
    [Required]
    public string? UserId { get; set; }
    [Required]
    public int? ProductId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }

    
}