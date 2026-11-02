using WepApi.Helpers;
using WepApi.Models;

namespace WepApi.Repository;

public interface IUserRepository
{
    Task<PageResult<AspNetUsers>> GetUserByPageAsync(int page = 1, int pageSize = 10);
    Task<List<AspNetUsers>> GetUsersAsync();
    Task<AspNetUsers?> GetUserByIdAsync(string userId);
    Task<AspNetUsers?> GetUserByNameAsync(string username);
    Task<AspNetUsers?> GetUserByEmailAsync(string email);
    Task UpdateUserAsync(AspNetUsers user);
    Task<AspNetUsers?> Login(AspNetUsers request);
    Task<AspNetUsers> Register(AspNetUsers request);
}