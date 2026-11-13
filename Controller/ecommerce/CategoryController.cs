using Microsoft.AspNetCore.Mvc;
using WepApi.DTOs.ecommerce.Request;
using WepApi.Helpers;
using WepApi.Service.ecommerce.Interface;

namespace WepApi.Controller.ecommerce;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoriesByPageAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var data = await _service.GetCategoriesByPageAsync(page, pageSize, cancellationToken);
        return ApiResponse.Success(data, "Categories retrieved successfully");
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var data = await _service.GetAllCategoriesAsync(cancellationToken);
        return ApiResponse.Success(data, "All categories retrieved successfully");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var data = await _service.GetByIdAsync(id, cancellationToken);
        if (data == null)
        {
            return NotFound(ApiResponse.Error($"Category with ID {id} not found.", StatusCodes.Status404NotFound));
        }
        return ApiResponse.Success(data, "Category retrieved successfully");
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CategoriesRequest req, CancellationToken cancellationToken = default)
    {
        var data = await _service.CreateAsync(req, cancellationToken);
        return ApiResponse.Success(data, "Category created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CategoriesRequest req, CancellationToken cancellationToken = default)
    {
        var data = await _service.UpdateAsync(id, req, cancellationToken);
        return ApiResponse.Success(data, "Category updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return ApiResponse.Success(result, "Category deleted successfully");
    }
}
