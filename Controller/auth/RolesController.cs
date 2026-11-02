using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WepApi.DTOs.Request;
using WepApi.Helpers;
using WepApi.Service.Interface;

namespace WepApi.Controller;
[ApiController]
[Route("api/roles")]
//[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [Authorize(Policy = "CanView")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _roleService.GetAllAsync();
        return ApiResponse.Success(data);
    }


    [Authorize(Policy = "CanView")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var data = await _roleService.GetByIdAsync(id);
        return ApiResponse.Success(data!);
    }
    [Authorize(Policy = "CanCreate")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleRequest request)
    {
        var data = await _roleService.CreateAsync(request);
        return ApiResponse.Success(data);
    }
        
    [Authorize(Policy = "CanUpdate")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] RoleRequest request)
    {
        var data = await _roleService.UpdateAsync(id, request);
        return ApiResponse.Success("Role Updated Success");
    }
    [Authorize(Policy = "CanDelete")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _roleService.DeleteAsync(id);
        return ApiResponse.Success(new { });
    }
}