using WepApi.Data;
using WepApi.Helpers;
using WepApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WepApi.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
       public async Task<PageResult<AspNetUsers>> GetUserByPageAsync( int page = 1, int pageSize = 10)
        {
            var data = await _dbContext.Users
                .OrderBy(u => u.Id)
                .ToPagedListAsync(page, pageSize);
            return data;
        }

        public async Task<List<AspNetUsers>> GetUsersAsync()
        {
            var data = await _dbContext.Users.ToListAsync();
            return data;
        }

        public async Task<AspNetUsers?> GetUserByIdAsync(string userId)
        {
            var data = await _dbContext.Users
                .FirstOrDefaultAsync(x=> x.Id == userId);
            return data;
        } 

        public async Task<AspNetUsers?> GetUserByNameAsync(string username)
        {
            username = username.Trim().ToLowerInvariant();
            var data = await _dbContext.Users
                .FirstOrDefaultAsync(x=> x.UserName == username);
            return data;
        } 

        public async Task<AspNetUsers?> GetUserByEmailAsync(string email)
        {
            var emailExists = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Email == email.Trim().ToLowerInvariant());
            return emailExists;
        }

        public async Task<AspNetUsers?> Login(AspNetUsers request)
        {
            var data = await _dbContext.Users
                .FirstOrDefaultAsync(x=> x.UserName == request.UserName && x.PasswordHash == request.PasswordHash);
            return data;
        }

        public async Task<AspNetUsers> Register(AspNetUsers request)
        {
            await _dbContext.Users.AddAsync(request);
            await _dbContext.SaveChangesAsync();
            return request;
        }

        public async Task UpdateUserAsync(AspNetUsers user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    
    
    
}