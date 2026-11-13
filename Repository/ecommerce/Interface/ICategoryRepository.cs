using WepApi.Helpers;
using WepApi.Models;

namespace WepApi.Repository.ecommerce.Interface;

public interface ICategoryRepository
{
    Task<PageResult<Categories>> GetCategoriesByPageAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<List<Categories>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Categories?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Categories> CreateAsync(Categories category, CancellationToken cancellationToken = default);
    Task<Categories> UpdateAsync(Categories category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Categories category, CancellationToken cancellationToken = default);
}
