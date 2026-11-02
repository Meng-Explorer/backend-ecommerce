namespace WepApi.Models;

public class CartItems
{
    public int Id { get; set; }
    
    public Carts? Cart { get; set; }
    public int CartId { get; set; }
    
    public Products? Product { get; set; }
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
}