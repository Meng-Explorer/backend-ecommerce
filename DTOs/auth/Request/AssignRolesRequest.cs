namespace WepApi.DTOs.Request;

public class AssignRolesRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<string> RoleIds { get; set; } = [];
}