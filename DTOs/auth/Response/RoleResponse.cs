namespace WepApi.DTOs.Response;

public class RoleResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NormalizedName { get; set; }
}