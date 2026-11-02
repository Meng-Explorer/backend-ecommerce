using Microsoft.EntityFrameworkCore;
using WepApi.Data;
using WepApi.Models;

namespace WepApi.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext  _dbContext;
    public RoleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<AspNetRoles>> GetAllAsync()
    {
        return await _dbContext.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<AspNetRoles?> GetByIdAsync(string id)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(x=> x.Id == id);
    }

    public async Task<AspNetRoles?> GetByNameAsync(string name)
    {
        return await _dbContext.Roles
            .FirstOrDefaultAsync(x=> x.Name == name);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbContext.Roles
            .CountAsync(x => x.Name == name) > 0;
    }

    public async Task<bool> ExistsByNameAsync(string name, string excludeId)
    {
        return await _dbContext.Roles
            .CountAsync(x => x.Name == name && x.Id != excludeId) > 0;
    }

    public async Task AddAsync(AspNetRoles role)
    {
        await _dbContext.Roles.AddAsync(role);
    }

    public async Task Update(AspNetRoles role)
    {
        _dbContext.Roles.Update(role);
    }

    public void Delete(AspNetRoles role)
    {
        _dbContext.Roles.Remove(role);
    }

    public async Task<bool> HasUsersAsync(string roleId)
    {
        return await _dbContext.UserRoles
            .CountAsync(x => x.RoleId == roleId) > 0;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
    
    
}