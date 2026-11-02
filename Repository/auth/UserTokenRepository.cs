using Microsoft.EntityFrameworkCore;
using WepApi.Data;
using WepApi.Models;

namespace WepApi.Repository;

public class UserTokenRepository : IUserTokenRepository
{
    private readonly ApplicationDbContext _dbContext;
    public  UserTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<AspNetUserTokens>> GetByUserIdAsync(string userId)
    {
        return await _dbContext.UserTokens
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<AspNetUserTokens?> GetAsync(string userId, string loginProvider, string name)
    {
        return await _dbContext.UserTokens
            .FirstOrDefaultAsync(x => x.UserId == userId && x.LoginProvider == loginProvider && x.Name == name);
    }

    public async Task AddAsync(AspNetUserTokens token)
    {
        await _dbContext.UserTokens.AddAsync(token);
    }

    public void Delete(AspNetUserTokens token)
    {
        _dbContext.UserTokens.Remove(token);
    }

    public async Task<AspNetUserTokens?> GetByProviderAndNameAsync(string loginProvider, string name)
    {
        return await _dbContext.UserTokens
            .FirstOrDefaultAsync(x => x.LoginProvider == loginProvider && x.Name == name);
    }

    public async Task<List<AspNetUserTokens>> GetByUserAndProviderAsync(string userId, string loginProvider)
    {
        return await _dbContext.UserTokens
            .Where(x => x.UserId == userId && x.LoginProvider == loginProvider)
            .ToListAsync();
    }

    public void DeleteRange(IEnumerable<AspNetUserTokens> tokens)
    {
        _dbContext.UserTokens.RemoveRange(tokens);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
    
    
}