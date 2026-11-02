using Microsoft.EntityFrameworkCore;
using WepApi.Data;
using WepApi.Models;

namespace WepApi.Repository;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly ApplicationDbContext _dbContext;
    public UserRoleRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<AspNetRoles>> GetUserRolesAsync(string userId)
    {
        return await _dbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Role)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<AspNetUserRoles?> GetUserRoleAsync(string userId,string roleId)
    {
        return await _dbContext.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
    }
    public async Task<List<AspNetUserRoles>> GetUserRoleEntitiesASync(string userId)
    {
        return await _dbContext.UserRoles
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
    public async Task<bool> HashRoleAsync(string userId,string roleId)
    {
        return await _dbContext.UserRoles
            .CountAsync(x => x.UserId == userId && x.RoleId == roleId) > 0;
    }

    public async Task AddUserRoleAsync(AspNetUserRoles userRole)
    {
        await _dbContext.UserRoles.AddAsync(userRole);
    } 
    public async Task AddUserRolesAsync(IEnumerable<AspNetUserRoles> userRoles)
    {
        await _dbContext.UserRoles.AddRangeAsync(userRoles);
    }

    public void RemoveUserRole(AspNetUserRoles userRole)
    {
        _dbContext.UserRoles.Remove(userRole);
    }

    public void RemoveUserRoles(IEnumerable<AspNetUserRoles> userRoles)
    {
        _dbContext.UserRoles.RemoveRange(userRoles);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
    
    
}