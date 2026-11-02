using WepApi.DTOs.Request;
using WepApi.DTOs.Response;

namespace WepApi.Service.Interface;

public interface IUserClaimService
{
    Task<List<UserClaimResponse>> GetByUserIdAsync(string userId);
    Task<UserClaimResponse> AddAsync(UserClaimRequest request);
    Task<bool> DeleteAsync(int id);
}