using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WepApi.DTOs.Request;
using WepApi.Helpers;
using WepApi.Service.Interface;

namespace WepApi.Controller;
[ApiController]
[Route("api/user-logins")]
[Authorize]
public class UserLoginController : ControllerBase
{
    private readonly IUserLoginService _userLoginService;
    public UserLoginController(IUserLoginService userLoginService)
    {
        _userLoginService = userLoginService;
    }
    [Authorize(Policy = "CanView")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var data = await _userLoginService.GetByUserIdAsync(userId);
        return ApiResponse.Success(data, "User logins retrieved successfully");
    }
    [Authorize(Policy = "CanCreate")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserLoginRequest request)
    {
        var data = await _userLoginService.AddAsync(request);
        return ApiResponse.Success(data, "User login linked successfully");
    }
    [Authorize(Policy = "CanDelete")]
    [HttpDelete("{loginProvider}/{providerKey}")]
    public async Task<IActionResult> Remove(string loginProvider, string providerKey)
    {
        await _userLoginService.RemoveAsync(loginProvider, providerKey);
        return ApiResponse.Success(new { }, "User login removed successfully");
    }
    
}