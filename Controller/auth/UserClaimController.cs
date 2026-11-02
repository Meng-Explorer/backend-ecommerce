using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WepApi.DTOs.Request;
using WepApi.Helpers;
using WepApi.Service.Interface;

namespace WepApi.Controller;
[ApiController]
[Route("api/user-claims")]
[Authorize]
public class UserClaimController :ControllerBase
{
    private readonly IUserClaimService _userClaimService;
    public UserClaimController(IUserClaimService userClaimService)
    {
        _userClaimService = userClaimService;
    }
    [Authorize(Policy = "CanView")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var data = await _userClaimService.GetByUserIdAsync(userId);
        return ApiResponse.Success(data, "User claims retrieved successfully");
    }
    [Authorize(Policy = "CanCreate")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserClaimRequest request)
    {
        var data = await _userClaimService.AddAsync(request);
        return ApiResponse.Success(data, "User claim added successfully");
    }
    [Authorize(Policy = "CanDelete")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userClaimService.DeleteAsync(id);
        return ApiResponse.Success(new { }, "User claim removed successfully");
    }
}