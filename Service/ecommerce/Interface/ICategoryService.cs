using WepApi.DTOs.ecommerce.Request;
using WepApi.DTOs.ecommerce.Response;
using WepApi.Helpers;

namespace WepApi.Service.ecommerce.Interface;

public interface ICategoryService
{
    Task<PageResult<CategoriesResponse>> GetCategoriesByPageAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<List<CategoriesResponse>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoriesResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoriesResponse> CreateAsync(CategoriesRequest req, CancellationToken cancellationToken = default);
    Task<CategoriesResponse> UpdateAsync(int id, CategoriesRequest req, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
