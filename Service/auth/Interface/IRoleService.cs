using WepApi.DTOs.Request;
using WepApi.DTOs.Response;

namespace WepApi.Service.Interface;

public interface IRoleService
{
    Task<List<RoleResponse>> GetAllAsync();
    Task<RoleResponse?> GetByIdAsync(string id);
    Task<RoleResponse> CreateAsync(RoleRequest request);
    Task<RoleResponse?> UpdateAsync(string id, RoleRequest request);
    Task<bool> DeleteAsync(string id);
}