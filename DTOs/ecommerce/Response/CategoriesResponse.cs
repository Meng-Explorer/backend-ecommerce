namespace WepApi.DTOs.ecommerce.Response;

public class CategoriesResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}