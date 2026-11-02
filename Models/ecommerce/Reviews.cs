using System;

namespace WepApi.Models;

public class Reviews
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Products? Product { get; set; }
    public int ProductId { get; set; }
    
    public AspNetUsers? User { get; set; }
    public string? UserId { get; set; }
}