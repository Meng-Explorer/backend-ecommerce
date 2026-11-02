namespace WepApi.DTOs.Response;

public class UserLoginResponse
{
    public string UserId { get; set; } = string.Empty;
    public string LoginProvider { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
}