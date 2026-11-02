using System.ComponentModel.DataAnnotations;

namespace WepApi.DTOs.ecommerce.Request;

public class CartItemRequest
{
    [Required]
    public int? CartId { get; set; }
    [Required]
    public int? ProductId { get; set; }
    public int? Quantity { get; set; }
}