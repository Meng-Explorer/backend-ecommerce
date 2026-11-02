using WepApi.Models;

namespace WepApi.Repository;

public interface IRoleRepository
{
    Task<List<AspNetRoles>> GetAllAsync();
    Task<AspNetRoles?> GetByIdAsync(string id);
    Task<AspNetRoles?> GetByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name, string excludeId);
    Task AddAsync(AspNetRoles role);
    Task Update(AspNetRoles role);
    void Delete(AspNetRoles role);
    Task<bool> HasUsersAsync(string roleId);
    Task<int> SaveChangesAsync();
}