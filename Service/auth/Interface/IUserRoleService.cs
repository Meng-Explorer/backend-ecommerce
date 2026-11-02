using WepApi.DTOs.Response;

namespace WepApi.Service.Interface;

public interface IUserRoleService
{
    Task<UserRoleResponse?> GetUserRolesAsync(string userId);
    Task<bool> AssignRoleAsync(string userId, string roleId);
    Task<bool> AssignRolesAsync(string userId, List<string> roleIds);
    Task<bool> RemoveRoleAsync(string userId, string roleId);
    Task<bool> RemoveRolesAsync(string userId, List<string> roleIds);
}