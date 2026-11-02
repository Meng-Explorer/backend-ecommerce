namespace WepApi.DTOs.Request;

public class RoleClaimRequest
{
    public string RoleId { get; set; } = string.Empty;
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}