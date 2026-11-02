using WepApi.Models;

namespace WepApi.DTOs.Response;

public class LoginResponse
{
    public UserResponse? User { get; set;}
    public string? Token { get; set;}
    public string? RefreshToken { get; set; }
    
    public List<RoleResponse> Roles { get; set; } = [];
}