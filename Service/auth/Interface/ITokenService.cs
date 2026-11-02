using WepApi.Models;

namespace WepApi.Service.Interface;

public interface ITokenService
{
    string CreateAccessToken(AspNetUsers user, IEnumerable<string> roles);
    Task<string> IssueRefreshTokenAsync(string userId);
    Task<(AspNetUsers User, string RefreshToken)?> RotateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task RevokeAllRefreshTokensAsync(string userId);
}