namespace WepApi.DTOs.Response;

public class RoleClaimResponse
{
    public int Id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}