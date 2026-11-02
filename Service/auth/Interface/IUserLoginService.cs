using WepApi.DTOs.Request;
using WepApi.DTOs.Response;

namespace WepApi.Service.Interface;

public interface IUserLoginService
{
    Task<List<UserLoginResponse>> GetByUserIdAsync(string userId);
    Task<UserLoginResponse> AddAsync(UserLoginRequest request);
    Task<bool> RemoveAsync(string loginProvider, string providerKey);
}