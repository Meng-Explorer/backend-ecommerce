using WepApi.DTOs.Request;
using WepApi.DTOs.Response;
using WepApi.Helpers;

namespace WepApi.Service.Interface;

public interface IUserService
{
    Task<PageResult<UserResponse>> GetUsersByPageAsync(int page = 1, int pageSize = 10);
    Task<List<UserResponse>> GetUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(string userId);
    Task<LoginResponse> Login(LoginRequest request);
    Task<UserResponse> Register(RegisterRequest request);
    Task<UserResponse> CreateUser(RegisterRequest request);
    Task<LoginResponse> ExternalLoginAsync(ExternalRequest request);
    Task<LoginResponse> ExternalLoginFromClaimsAsync(string provider, string providerKey, string email, string? firstName, string? lastName);
    Task<LoginResponse> RefreshAsync(string refreshToken);
    Task<string> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<bool> VerifyOtpAsync(VerifyOTP request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    Task LogoutAsync(string refreshToken);
    Task LogoutAllAsync(string userId);
}