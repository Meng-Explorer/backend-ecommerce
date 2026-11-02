using Microsoft.EntityFrameworkCore;
using WepApi.Data;
using WepApi.Models;

namespace WepApi.Repository;

public class UserLoginRepository : IUserLoginRepository
{
    private readonly ApplicationDbContext _dbContext;
    public UserLoginRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<AspNetUserLogins>> GetByUserIdAsync(string userId)
    {
        return await _dbContext.UserLogins
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<AspNetUserLogins?> GetAsync(string loginProvider, string providerKey)
    {
        return await _dbContext.UserLogins
            .FirstOrDefaultAsync(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
    }

    public async Task AddAsync(AspNetUserLogins login)
    {
        await _dbContext.UserLogins.AddAsync(login);
    }

    public void Delete(AspNetUserLogins login)
    {
        _dbContext.UserLogins.Remove(login);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
    
    
}