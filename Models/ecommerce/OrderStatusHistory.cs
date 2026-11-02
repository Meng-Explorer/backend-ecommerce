using System;

namespace WepApi.Models;

public class OrderStatusHistory
{
    public int Id { get; set; }
    public Orders? Order { get; set; }
    public int OrderId { get; set; }
    
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}