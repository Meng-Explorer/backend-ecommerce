using WepApi.Models;

namespace WepApi.Repository;

public interface IUserClaimRepository
{
    Task<List<AspNetUserClaims>> GetByUserIdAsync(string userId);
    Task<AspNetUserClaims?> GetByIdAsync(int id);
    Task AddAsync(AspNetUserClaims claim);
    void Delete(AspNetUserClaims claim);
    Task<int> SaveChangesAsync();
}