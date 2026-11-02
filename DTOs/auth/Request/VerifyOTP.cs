namespace WepApi.DTOs.Request;

public class VerifyOTP
{
    public string Email { get; set; } = null!;
    public string Otp { get; set; } = null!;
}