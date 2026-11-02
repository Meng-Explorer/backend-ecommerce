namespace WepApi.DTOs.Request;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string? Email {  get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
}