namespace WepApi.Models;

public class Products
{
    public int Id { get; set; }
    
    public string? Name { get; set; }
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Categories? Category { get; set; }
    public int? CategoryId { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    public virtual ICollection<Wishlists> Wishlists { get; set; } = new List<Wishlists>();
}