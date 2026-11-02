using System;
using System.Collections.Generic;

namespace WepApi.Models;

public class Carts
{
    public int Id { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public AspNetUsers? User { get; set; }
    public string? UserId { get; set; }

    public virtual ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
}