using WepApi.Models;

namespace WepApi.Repository;

public interface IUserRoleRepository
{
    Task<List<AspNetRoles>> GetUserRolesAsync(string userId);
    Task<AspNetUserRoles?> GetUserRoleAsync(string userId,string roleId);
    Task<List<AspNetUserRoles>> GetUserRoleEntitiesASync(string userId);
    Task<bool> HashRoleAsync(string userId,string roleId);
    Task AddUserRoleAsync(AspNetUserRoles userRole);
    Task AddUserRolesAsync(IEnumerable<AspNetUserRoles> userRoles);
    void RemoveUserRole(AspNetUserRoles userRole);
    void RemoveUserRoles(IEnumerable<AspNetUserRoles> userRoles);
    Task<int> SaveChangesAsync();
}