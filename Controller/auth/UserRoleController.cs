using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WepApi.DTOs.Request;
using WepApi.Helpers;
using WepApi.Service.Interface;

namespace WepApi.Controller;
[ApiController]
[Route("api/user-roles")]
[Authorize]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;
    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var result = await _userRoleService.GetUserRolesAsync(userId);
        return ApiResponse.Success(result!, "User roles retrieved successfully");
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var result = await _userRoleService.AssignRoleAsync(request.UserId, request.RoleId);
        return ApiResponse.Success(new { }, "Role assigned success");
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("assign-multiple")]
    public async Task<IActionResult> AssignRoles([FromBody] AssignRolesRequest request)
    {
        var result = await _userRoleService.AssignRolesAsync(request.UserId, request.RoleIds);
        return ApiResponse.Success(new { }, "Roles assigned success");
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request)
    {
        var result = await _userRoleService.RemoveRoleAsync(request.UserId, request.RoleId);
        return ApiResponse.Success(new { }, "Role removed success");
    }
        
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("remove-multiple")]
    public async Task<IActionResult> RemovesRole([FromBody] RemoveRolesRequest request)
    {
        var result = await _userRoleService.RemoveRolesAsync(request.UserId, request.RoleIds);
        return ApiResponse.Success(new { }, "Role removed success");
    }
}