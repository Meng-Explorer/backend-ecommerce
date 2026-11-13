using AutoMapper;
using WepApi.DTOs.ecommerce.Request;
using WepApi.DTOs.ecommerce.Response;
using WepApi.Exceptions;
using WepApi.Helpers;
using WepApi.Models;
using WepApi.Repository.ecommerce.Interface;
using WepApi.Service.ecommerce.Interface;

namespace WepApi.Service.ecommerce;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PageResult<CategoriesResponse>> GetCategoriesByPageAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page <= 0 || pageSize <= 0)
        {
            throw new ValidationException(new List<string> { "Page and PageSize must be greater than zero." });
        }

        var result = await _repository.GetCategoriesByPageAsync(page, pageSize, cancellationToken);
        if (result == null || result.Data == null)
        {
            throw new NotFoundException("No categories found.");
        }

        return new PageResult<CategoriesResponse>
        {
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalRecords = result.TotalRecords,
            TotalPages = result.TotalPages,
            Data = _mapper.Map<List<CategoriesResponse>>(result.Data)
        };
    }

    public async Task<List<CategoriesResponse>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var list = await _repository.GetAllCategoriesAsync(cancellationToken);
        return _mapper.Map<List<CategoriesResponse>>(list);
    }

    public async Task<CategoriesResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ValidationException(new List<string> { "Id must be greater than zero." });
        }

        var category = await _repository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category with ID {id} not found.");
        }

        return _mapper.Map<CategoriesResponse>(category);
    }

    public async Task<CategoriesResponse> CreateAsync(CategoriesRequest req, CancellationToken cancellationToken = default)
    {
        if (req == null)
        {
            throw new ArgumentNullException(nameof(req));
        }

        var category = _mapper.Map<Categories>(req);
        var created = await _repository.CreateAsync(category, cancellationToken);
        return _mapper.Map<CategoriesResponse>(created);
    }

    public async Task<CategoriesResponse> UpdateAsync(int id, CategoriesRequest req, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ValidationException(new List<string> { "Id must be greater than zero." });
        }

        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            throw new NotFoundException($"Category with ID {id} not found.");
        }

        _mapper.Map(req, existing);
        var updated = await _repository.UpdateAsync(existing, cancellationToken);
        return _mapper.Map<CategoriesResponse>(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ValidationException(new List<string> { "Id must be greater than zero." });
        }

        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            throw new NotFoundException($"Category with ID {id} not found.");
        }

        return await _repository.DeleteAsync(existing, cancellationToken);
    }
}
