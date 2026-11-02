namespace WepApi.DTOs.Request;

public class ExternalRequest
{
    public string Provider { get; set; } = string.Empty;
    public string? IdToken { get; set; }
    public string? AccessToken { get; set; }
    public string? Code { get; set; }
}