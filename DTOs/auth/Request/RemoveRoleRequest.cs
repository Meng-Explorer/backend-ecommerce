namespace WepApi.DTOs.Request;

public class RemoveRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
}