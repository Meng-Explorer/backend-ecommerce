using Microsoft.EntityFrameworkCore;
using WepApi.Data;
using WepApi.Models;

namespace WepApi.Repository;

public class RoleClaimRepository : IRoleClaimRepository
{
    private readonly ApplicationDbContext _dbContext;
    public RoleClaimRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<AspNetRoleClaims>> GetByRoleIdAsync(string roleId)
    {
        return await _dbContext.RoleClaims
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<AspNetRoleClaims?> GetByIdAsync(int id)
    {
        return await _dbContext.RoleClaims
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(AspNetRoleClaims claim)
    {
        await _dbContext.RoleClaims.AddAsync(claim);
    }

    public void Delete(AspNetRoleClaims claim)
    {
        _dbContext.RoleClaims.Remove(claim);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}