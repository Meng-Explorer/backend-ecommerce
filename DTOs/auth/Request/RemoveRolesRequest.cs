namespace WepApi.DTOs.Request;

public class RemoveRolesRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<string> RoleIds { get; set; } = [];
}