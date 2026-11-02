using WepApi.Models;

namespace WepApi.Repository;

public interface IUserTokenRepository
{
    Task<List<AspNetUserTokens>> GetByUserIdAsync(string userId);
    Task<AspNetUserTokens?> GetAsync(string userId, string loginProvider, string name);
    Task AddAsync(AspNetUserTokens token);
    void Delete(AspNetUserTokens token);
    Task<int> SaveChangesAsync();
    Task<AspNetUserTokens?> GetByProviderAndNameAsync(string loginProvider, string name);
    Task<List<AspNetUserTokens>> GetByUserAndProviderAsync(string userId, string loginProvider);
    void DeleteRange(IEnumerable<AspNetUserTokens> tokens);
}