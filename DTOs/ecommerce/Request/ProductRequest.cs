using System.ComponentModel.DataAnnotations;

namespace WepApi.DTOs.ecommerce.Request;

public class ProductRequest
{
    [Required]
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    // ADI-TSH-BLK-M
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
}