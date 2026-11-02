namespace WepApi.DTOs.Request;

public class AssignRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}