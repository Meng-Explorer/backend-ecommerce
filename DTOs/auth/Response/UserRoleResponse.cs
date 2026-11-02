namespace WepApi.DTOs.Response;

public class UserRoleResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string FullName { get; set; } = string.Empty;
    public List<RoleResponse> Roles { get; set; } = [];
}