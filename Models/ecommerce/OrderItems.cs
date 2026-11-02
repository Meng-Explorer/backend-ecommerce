namespace WepApi.Models;

public class OrderItems
{
    public int Id { get; set; }
    public Orders? Order { get; set; }
    public int OrderId { get; set; }
    
    public Products? Product { get; set; }
    public int ProductId { get; set; }
    public string? PhoneNameSnapshot { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; } 
}