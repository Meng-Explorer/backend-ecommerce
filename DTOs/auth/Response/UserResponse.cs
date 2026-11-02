namespace WepApi.DTOs.Response;

public class UserResponse
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? Status { get; set; }
    public bool IsSuperUser { get; set; }
}