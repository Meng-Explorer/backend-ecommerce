using WepApi.DTOs.Request;
using WepApi.DTOs.Response;

namespace WepApi.Service.Interface;

public interface IRoleClaimService
{
    Task<List<RoleClaimResponse>> GetByRoleIdAsync(string roleId);
    Task<RoleClaimResponse> AddAsync(RoleClaimRequest request);
    Task<bool> DeleteAsync(int id);
}