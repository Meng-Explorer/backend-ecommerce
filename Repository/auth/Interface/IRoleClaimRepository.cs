using WepApi.Models;

namespace WepApi.Repository;

public interface IRoleClaimRepository
{
    Task<List<AspNetRoleClaims>> GetByRoleIdAsync(string roleId);
    Task<AspNetRoleClaims?> GetByIdAsync(int id);
    Task AddAsync(AspNetRoleClaims claim);
    void Delete(AspNetRoleClaims claim);
    Task<int> SaveChangesAsync();
}