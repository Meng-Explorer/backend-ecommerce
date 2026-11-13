using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WepApi.DTOs.Request;
using WepApi.Helpers;
using WepApi.Service.Interface;

namespace WepApi.Controller;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Policy = "CanView")]
        [HttpGet("page")]
        public async Task<IActionResult> GetUsersByPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _userService.GetUsersByPageAsync(page, pageSize);
            return ApiResponse.Success(data, "Users retrieved successfully");
        }

        [Authorize(Policy = "CanView")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var data = await _userService.GetUsersAsync();
            return ApiResponse.Success(data, "All users retrieved successfully");
        }

        [Authorize(Policy = "CanView")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var data = await _userService.GetUserByIdAsync(id);

            if (data == null)
            {
                return NotFound(ApiResponse.Error("User not found", StatusCodes.Status404NotFound));
            }

            return ApiResponse.Success(data, "User retrieved successfully");
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var data = await _userService.Register(request);
            return ApiResponse.Success(data, "Register Success");
        }
        
        [Authorize(Policy = "CanCreate")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest request)
        {
            var data = await _userService.CreateUser(request);
            return ApiResponse.Success(data, "User created successfully by Admin");
        }
        
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var data = await _userService.Login(request);
            return ApiResponse.Success(data, "Login Success");
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Provider))
            {
                return BadRequest(ApiResponse.Error("Provider is required"));
            }

            var data = await _userService.ExternalLoginAsync(request);
            return ApiResponse.Success(data, "Login Success");
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("login-google")]
        public async Task<IActionResult> GoogleLogin([FromBody] ExternalRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Provider))
            {
                request.Provider = "google";
            }

            if (string.IsNullOrWhiteSpace(request.IdToken) && string.IsNullOrWhiteSpace(request.AccessToken))
            {
                return BadRequest(ApiResponse.Error("Google IdToken or AccessToken is required"));
            }

            var data = await _userService.ExternalLoginAsync(request);
            return ApiResponse.Success(data, "Google Login Success");
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("login-github")]
        public async Task<IActionResult> GithubLogin([FromBody] ExternalRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Provider))
            {
                request.Provider = "github";
            }

            if (string.IsNullOrWhiteSpace(request.AccessToken) && string.IsNullOrWhiteSpace(request.IdToken) && string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest(ApiResponse.Error("GitHub AccessToken or Code is required"));
            }

            var data = await _userService.ExternalLoginAsync(request);
            return ApiResponse.Success(data, "GitHub Login Success");
        }

        [AllowAnonymous]
        [HttpGet("external-login/{provider}")]
        public async Task<IActionResult> ExternalLoginChallenge(
            string provider,
            [FromServices] IAuthenticationSchemeProvider schemeProvider,
            [FromQuery] string? returnUrl = null)
        {
            var scheme = provider.ToLowerInvariant() switch
            {
                "google" => "Google",
                "github" => "GitHub",
                _ => null
            };

            if (scheme == null)
            {
                return BadRequest(ApiResponse.Error("Unsupported provider. Supported providers are 'google' and 'github'."));
            }

            var registeredScheme = await schemeProvider.GetSchemeAsync(scheme);
            if (registeredScheme == null)
            {
                return BadRequest(ApiResponse.Error($"{scheme} authentication is not configured on the server. Please configure ClientId and ClientSecret."));
            }

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items["LoginProvider"] = scheme;
            return Challenge(properties, scheme);
        }

        [AllowAnonymous]
        [HttpGet("external-callback")]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery] string? returnUrl = null)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                return BadRequest(ApiResponse.Error("External authentication failed or was cancelled."));
            }

            var provider = (authenticateResult.Properties != null && authenticateResult.Properties.Items.TryGetValue("LoginProvider", out var p) && !string.IsNullOrWhiteSpace(p))
                ? p
                : "External";

            var providerKey = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? authenticateResult.Principal.FindFirstValue("sub")
                              ?? authenticateResult.Principal.FindFirstValue("id");

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
            var firstName = authenticateResult.Principal.FindFirstValue(ClaimTypes.GivenName) ?? name;
            var lastName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Surname);

            if (string.IsNullOrWhiteSpace(providerKey))
            {
                return BadRequest(ApiResponse.Error("Could not retrieve provider key from external login."));
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                email = authenticateResult.Principal.Claims.FirstOrDefault(c => c.Type.Contains("email", StringComparison.OrdinalIgnoreCase))?.Value;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                email = $"{providerKey}@{provider.ToLowerInvariant()}.user";
            }

            var data = await _userService.ExternalLoginFromClaimsAsync(provider, providerKey, email, firstName, lastName);

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Uri.IsWellFormedUriString(returnUrl, UriKind.RelativeOrAbsolute))
            {
                var redirectUri = $"{returnUrl}?token={Uri.EscapeDataString(data.Token ?? string.Empty)}&refreshToken={Uri.EscapeDataString(data.RefreshToken ?? string.Empty)}";
                return Redirect(redirectUri);
            }

            return ApiResponse.Success(data, $"{provider} login successful");
        }
        
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(ApiResponse.Error("RefreshToken is required"));
            }

            var data = await _userService.RefreshAsync(request.RefreshToken);
            return ApiResponse.Success(data, "Token refreshed");
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            await _userService.LogoutAsync(request.RefreshToken);
            return ApiResponse.Success(new { }, "Logged out");
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse.Error("Invalid token", StatusCodes.Status401Unauthorized));
            }

            await _userService.LogoutAllAsync(userId);
            return ApiResponse.Success(new { }, "All sessions revoked");
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var message = await _userService.ForgotPasswordAsync(request);
            return ApiResponse.Success(new { }, message);
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOTP request)
        {
            var isVerified = await _userService.VerifyOtpAsync(request);
            return ApiResponse.Success(new { verified = isVerified }, "OTP verified successfully");
        }

        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var isReset = await _userService.ResetPasswordAsync(request);
            return ApiResponse.Success(new { reset = isReset }, "Password has been reset successfully");
        }

        [Authorize]
        [HttpGet("getProfile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var fullName = User.FindFirstValue("FullName");
            var roles = User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList();
            return ApiResponse.Success(new
            {
                userId,
                username,
                email,
                fullName,
                roles
            });
        }
}