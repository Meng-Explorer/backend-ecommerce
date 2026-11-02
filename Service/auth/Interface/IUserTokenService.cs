using WepApi.DTOs.Request;
using WepApi.DTOs.Response;

namespace WepApi.Service.Interface;

public interface IUserTokenService
{
    Task<List<UserTokenResponse>> GetByUserIdAsync(string userId);
    Task<UserTokenResponse> SetTokenAsync(UserTokenRequest request);
    Task<bool> RemoveAsync(string userId, string loginProvider, string name);
}