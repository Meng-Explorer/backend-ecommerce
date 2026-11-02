namespace WepApi.DTOs.Request;

public class UserClaimRequest
{
    public string UserId { get; set; } = string.Empty;
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}