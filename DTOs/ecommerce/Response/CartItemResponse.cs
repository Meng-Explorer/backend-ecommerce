namespace WepApi.DTOs.ecommerce.Response;

public class CartItemResponse
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public ProductResponse? Products { get; set; }
    public CartResponse? Cart { get; set; }
}