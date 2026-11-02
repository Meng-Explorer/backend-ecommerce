using Microsoft.EntityFrameworkCore;
using WepApi.Data;
using WepApi.Models;

namespace WepApi.Repository;

public class UserClaimRepository : IUserClaimRepository
{
    private readonly ApplicationDbContext  _dbContext;
    public UserClaimRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<List<AspNetUserClaims>> GetByUserIdAsync(string userId)
    {
        return await _dbContext.UserClaims
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<AspNetUserClaims?> GetByIdAsync(int id)
    {
        return await _dbContext.UserClaims
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(AspNetUserClaims claim)
    {
        await _dbContext.UserClaims.AddAsync(claim);
    }

    public void Delete(AspNetUserClaims claim)
    {
        _dbContext.UserClaims.Remove(claim);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}