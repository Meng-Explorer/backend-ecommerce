using WepApi.Models;

namespace WepApi.Repository;

public interface IUserLoginRepository
{
    Task<List<AspNetUserLogins>> GetByUserIdAsync(string userId);
    Task<AspNetUserLogins?> GetAsync(string loginProvider, string providerKey);
    Task AddAsync(AspNetUserLogins login);
    void Delete(AspNetUserLogins login);
    Task<int> SaveChangesAsync();
}