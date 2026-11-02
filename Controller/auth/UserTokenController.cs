using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WepApi.DTOs.Request;
using WepApi.Helpers;
using WepApi.Service.Interface;

namespace WepApi.Controller;
[ApiController]
[Route("api/user-tokens")]
[Authorize]
public class UserTokenController : ControllerBase
{
    
    private readonly IUserTokenService _userTokenService;
    public UserTokenController(IUserTokenService userTokenService)
    {
        _userTokenService = userTokenService;
    }
    [Authorize(Policy = "CanView")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var data = await _userTokenService.GetByUserIdAsync(userId);
        return ApiResponse.Success(data, "User tokens retrieved successfully");
    }
        
    [Authorize(Policy = "CanCreate")]
    [HttpPost]
    public async Task<IActionResult> Set([FromBody] UserTokenRequest request)
    {
        var data = await _userTokenService.SetTokenAsync(request);
        return ApiResponse.Success(data, "User token saved successfully");
    }
    [Authorize(Policy = "CanUpdate")]
    [HttpDelete("{userId}/{loginProvider}/{name}")]
    public async Task<IActionResult> Remove(string userId, string loginProvider, string name)
    {
        await _userTokenService.RemoveAsync(userId, loginProvider, name);
        return ApiResponse.Success(new { }, "User token removed successfully");
    }
}