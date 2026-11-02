using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WepApi.DTOs.Request;
using WepApi.Helpers;
using WepApi.Service.Interface;

namespace WepApi.Controller;
[ApiController]
[Route("api/role-claims")]
[Authorize]
public class RoleClaimController :ControllerBase
{
    private readonly IRoleClaimService _roleClaimService;
    public RoleClaimController(IRoleClaimService roleClaimService)
    {
        _roleClaimService = roleClaimService;
    }
    [Authorize(Policy = "CanView")]
    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetByRoleId(string roleId)
    {
        var data = await _roleClaimService.GetByRoleIdAsync(roleId);
        return ApiResponse.Success(data, "Role claims retrieved successfully");
    }
    [Authorize(Policy = "CanCreate")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleClaimRequest request)
    {
        var data = await _roleClaimService.AddAsync(request);
        return ApiResponse.Success(data, "Role claim added successfully");
    }
    [Authorize(Policy = "CanDelete")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _roleClaimService.DeleteAsync(id);
        return ApiResponse.Success(new { }, "Role claim removed successfully");
    }
}