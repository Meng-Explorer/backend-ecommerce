using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using WepApi.DTOs.Request;
using WepApi.DTOs.Response;
using WepApi.Exceptions;
using WepApi.Helpers;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Service.Interface;

namespace WepApi.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserRoleRepository  _userRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserLoginRepository _userLoginRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<AspNetUsers> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repository,
        IMapper mapper,
        IUserRoleRepository userRoleRepository,
        IRoleRepository roleRepository,
        IUserLoginRepository userLoginRepository,
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        IMemoryCache memoryCache,
        ILogger<UserService> logger,
        IConfiguration configuration,
        PasswordHasher<AspNetUsers> passwordHasher)
    {
        _repository = repository;
        _mapper = mapper;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
        _userLoginRepository = userLoginRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
        _memoryCache = memoryCache;
    }
    
    public async Task<PageResult<UserResponse>> GetUsersByPageAsync(int page = 1, int pageSize = 10)
        {
            var user = await _repository.GetUserByPageAsync(page, pageSize);
            if(user == null)
            {
                throw new NotFoundException("Data Not found");
            }
            return new PageResult<UserResponse>
            {
                PageNumber = user.PageNumber,
                PageSize = user.PageSize,
                TotalRecords = user.TotalRecords,
                TotalPages = user.TotalPages,
                Data = _mapper.Map<List<UserResponse>>(user.Data)
            };

                
        }
        private static bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email.Trim());
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim();
        }
        
        private static bool IsValidOtp(string? otp)
        {
            return !string.IsNullOrWhiteSpace(otp)
                   && otp.Trim().Length == 6
                   && Regex.IsMatch(otp.Trim(), "^\\d{6}$");
        }

        private static bool IsValidPassword(string? password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            if (request == null || !IsValidEmail(request.Email))
            {
                throw new BadRequestException("A valid email is required");
            }

            var email = NormalizeEmail(request.Email).ToLowerInvariant();
            var user = await _repository.GetUserByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("Password reset requested for unknown email {Email}", email);
                throw new NotFoundException("Email not found in our system.");
            }

            var otp = new Random().Next(100000, 999999).ToString();
            _memoryCache.Set($"OTP_{email}", otp, TimeSpan.FromMinutes(5));

            string emailBody = $@"
                <h3>Password Reset Request</h3>
                <p>Your OTP is: <strong>{otp}</strong></p>
                <p>This code will expire in 5 minutes.</p>";

            try
            {
                await SendEmailAsync(email, "Your OTP Code", emailBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset OTP to {Email}", email);
                string errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BadRequestException($"SMTP Error: {errorMessage}");
            }

            _logger.LogInformation("Successfully sent OTP for {Email}", email);
            return "If the email is registered, an OTP has been sent.";
        }
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var portValue = _configuration["EmailSettings:Port"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var senderName = _configuration["EmailSettings:SenderName"];

            if (string.IsNullOrWhiteSpace(smtpServer)
                || string.IsNullOrWhiteSpace(senderEmail)
                || string.IsNullOrWhiteSpace(senderPassword)
                || !int.TryParse(portValue, out var port))
            {
                throw new InvalidOperationException("EmailSettings are not configured for password reset.");
            }

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName ?? "Support"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }

        public async Task<bool> VerifyOtpAsync(VerifyOTP request)
        {
            if (request == null || !IsValidEmail(request.Email) || !IsValidOtp(request.Otp))
            {
                throw new BadRequestException("A valid email and 6-digit OTP are required");
            }

            var email = NormalizeEmail(request.Email).ToLowerInvariant();
            var otp = request.Otp.Trim();
            if (_memoryCache.TryGetValue($"OTP_{email}", out string? storedOtp))
            {
                if (storedOtp == otp)
                {
                    return true;
                }
            }

            throw new BadRequestException("Invalid or expired OTP");
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (request == null || !IsValidEmail(request.Email) || !IsValidOtp(request.Otp) || !IsValidPassword(request.NewPassword))
            {
                throw new BadRequestException("A valid email, 6-digit OTP, and a new password with at least 8 characters are required");
            }

            var email = NormalizeEmail(request.Email).ToLowerInvariant();
            var otp = request.Otp.Trim();
            if (!_memoryCache.TryGetValue($"OTP_{email}", out string? storedOtp) || storedOtp != otp)
            {
                throw new BadRequestException("Invalid or expired OTP");
            }

            var user = await _repository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            user.PasswordHash = HashPassword(user, request.NewPassword.Trim());
            await _repository.UpdateUserAsync(user);
            _memoryCache.Remove($"OTP_{email}");
            return true;
        }

        public async Task<List<UserResponse>> GetUsersAsync()
        {
            var data = await _repository.GetUsersAsync();
            return _mapper.Map<List<UserResponse>>(data);
        }

        public async Task<UserResponse?> GetUserByIdAsync(string userId)
        {
            var data = await _repository.GetUserByIdAsync(userId);
            return _mapper?.Map<UserResponse>(data);
        } 

        private string HashPassword(AspNetUsers user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        private PasswordVerificationResult VerificationResult(AspNetUsers user, string password)
        {
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return PasswordVerificationResult.Failed;
            }
            return _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
              );
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var identifier = request.Username.Trim();
            var user = await _repository.GetUserByNameAsync(identifier);

            if (user == null && !string.IsNullOrWhiteSpace(identifier))
            {
                user = await _repository.GetUserByEmailAsync(identifier);
            }

            if(user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }
            if(string.Equals(user.Status, "Inactive", StringComparison.OrdinalIgnoreCase)
                || (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow))
            {
                throw new BadRequestException("Account is inactive");
            }
            var passwordValid = VerificationResult(user, request.Password);
            if(passwordValid == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid password or username");
            }
            var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id);
            var roles = userRoles.Select(x => x.Name).ToList();

            if (!roles.Any() && !user.IsSuperUser)
            {
                var defaultRole = await _roleRepository.GetByNameAsync("User");
                if (defaultRole == null)
                {
                    defaultRole = new AspNetRoles { Name = "User", NormalizedName = "USER" };
                    await _roleRepository.AddAsync(defaultRole);
                    await _roleRepository.SaveChangesAsync();
                }

                await _userRoleRepository.AddUserRoleAsync(new AspNetUserRoles
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                });
                await _userRoleRepository.SaveChangesAsync();

                roles.Add("User");
                userRoles.Add(defaultRole);
            }

            var token = _tokenService.CreateAccessToken(user, roles!);
            var refreshToken = await _tokenService.IssueRefreshTokenAsync(user.Id);
            var result = new LoginResponse()
            {
                User = _mapper.Map<UserResponse>(user),
                Token = token,
                RefreshToken = refreshToken,
                Roles = _mapper.Map<List<RoleResponse>>(userRoles)

            };
            return result;

        }
        
        public async Task<UserResponse> Register(RegisterRequest request)
        {
            var username = request.Username.Trim().ToLowerInvariant();
            var user = await _repository.GetUserByNameAsync(request.Username);
            if (user != null) throw new BadRequestException("Username already Exist");
            
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExists = await _repository.GetUserByEmailAsync(request.Email);
                if (emailExists != null) throw new BadRequestException("Email already Exist");
            }
    
            var newUser = _mapper.Map<AspNetUsers>(request);
            newUser.UserName = username;
            if (!string.IsNullOrWhiteSpace(newUser.Email)) newUser.Email = newUser.Email.Trim().ToLowerInvariant();
            
            newUser.PasswordHash = HashPassword(newUser, request.Password);
    
            await _repository.Register(newUser);
    
            var defaultRole = await _roleRepository.GetByNameAsync("User");
            if (defaultRole == null)
            {
                defaultRole = new AspNetRoles { Name = "User", NormalizedName = "USER" };
                await _roleRepository.AddAsync(defaultRole);
                await _roleRepository.SaveChangesAsync();
            }

            await _userRoleRepository.AddUserRoleAsync(new AspNetUserRoles
            {
                UserId = newUser.Id,
                RoleId = defaultRole.Id
            });
            await _userRoleRepository.SaveChangesAsync();
    
            return _mapper.Map<UserResponse>(newUser);
        }

        public async Task<UserResponse> CreateUser(RegisterRequest request)
        {
            var username = request.Username.Trim().ToLowerInvariant();
            var user = await _repository.GetUserByNameAsync(request.Username);
            if (user != null) throw new BadRequestException("Username already Exist");
            
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var emailExists = await _repository.GetUserByEmailAsync(request.Email);
                if (emailExists != null) throw new BadRequestException("Email already Exist");
            }
    
            var newUser = _mapper.Map<AspNetUsers>(request);
            newUser.UserName = username;
            if (!string.IsNullOrWhiteSpace(newUser.Email)) newUser.Email = newUser.Email.Trim().ToLowerInvariant();
            
            newUser.PasswordHash = HashPassword(newUser, request.Password);
    
            await _repository.Register(newUser);
            return _mapper.Map<UserResponse>(newUser);
        }
        
        
        public async Task<LoginResponse> ExternalLoginAsync(ExternalRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Provider))
            {
                throw new BadRequestException("Provider is required");
            }

            ExternalAuthUser authUser;
            var providerLower = request.Provider.Trim().ToLowerInvariant();
            if (providerLower == "google")
            {
                authUser = await VerifyGoogleAsync(request);
            }
            else if (providerLower == "github")
            {
                authUser = await VerifyGithubAsync(request);
            }
            else
            {
                throw new BadRequestException($"Unsupported provider '{request.Provider}'. Supported providers are 'Google' and 'GitHub'.");
            }

            return await ProcessExternalUserAsync(authUser);
        }

        public async Task<LoginResponse> ExternalLoginFromClaimsAsync(string provider, string providerKey, string email, string? firstName, string? lastName)
        {
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerKey))
            {
                throw new BadRequestException("Provider and ProviderKey are required");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new BadRequestException("Email is required for external login");
            }

            var authUser = new ExternalAuthUser
            {
                Provider = provider,
                ProviderKey = providerKey,
                Email = email,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty
            };

            return await ProcessExternalUserAsync(authUser);
        }

        private async Task<LoginResponse> ProcessExternalUserAsync(ExternalAuthUser authUser)
        {
            var email = authUser.Email.Trim().ToLowerInvariant();

            // 1. Check if external login mapping exists in AspNetUserLogins
            AspNetUsers? user = null;
            var existingLogin = await _userLoginRepository.GetAsync(authUser.Provider, authUser.ProviderKey);
            if (existingLogin != null)
            {
                user = await _repository.GetUserByIdAsync(existingLogin.UserId);
            }

            // 2. If not found by provider mapping, check by email
            if (user == null)
            {
                user = await _repository.GetUserByEmailAsync(email);
                if (user != null)
                {
                    // Link provider to existing user
                    if (existingLogin == null)
                    {
                        await _userLoginRepository.AddAsync(new AspNetUserLogins
                        {
                            LoginProvider = authUser.Provider,
                            ProviderKey = authUser.ProviderKey,
                            ProviderDisplayName = authUser.Provider,
                            UserId = user.Id
                        });
                        await _userLoginRepository.SaveChangesAsync();
                    }
                }
                else
                {
                    // 3. User does not exist -> Auto-Register new user
                    var baseUsername = email.Split('@')[0];
                    var username = await GenerateUniqueUsernameAsync(baseUsername);

                    user = new AspNetUsers
                    {
                        Email = email,
                        EmailConfirmed = true,
                        UserName = username,
                        FirstName = authUser.FirstName,
                        LastName = authUser.LastName,
                        Status = "Active"
                    };

                    var dummyPassword = Guid.NewGuid().ToString("N") + "!Aa1";
                    user.PasswordHash = HashPassword(user, dummyPassword);

                    await _repository.Register(user);

                    // Assign Default Role ("User") if it exists
                    var defaultRole = await _roleRepository.GetByNameAsync("User");
                    if (defaultRole != null)
                    {
                        await _userRoleRepository.AddUserRoleAsync(new AspNetUserRoles
                        {
                            UserId = user.Id,
                            RoleId = defaultRole.Id
                        });
                        await _userRoleRepository.SaveChangesAsync();
                    }

                    // Link external login provider
                    await _userLoginRepository.AddAsync(new AspNetUserLogins
                    {
                        LoginProvider = authUser.Provider,
                        ProviderKey = authUser.ProviderKey,
                        ProviderDisplayName = authUser.Provider,
                        UserId = user.Id
                    });
                    await _userLoginRepository.SaveChangesAsync();
                }
            }

            // 4. Validate user status
            if (string.Equals(user.Status, "Inactive", StringComparison.OrdinalIgnoreCase)
                || (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow))
            {
                throw new BadRequestException("Account is inactive or locked out.");
            }

            // 5. Generate access and refresh tokens
            var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id);
            var roles = userRoles.Select(x => x.Name).ToList();

            var token = _tokenService.CreateAccessToken(user, roles!);
            var refreshToken = await _tokenService.IssueRefreshTokenAsync(user.Id);

            return new LoginResponse
            {
                User = _mapper.Map<UserResponse>(user),
                Token = token,
                RefreshToken = refreshToken,
                Roles = _mapper.Map<List<RoleResponse>>(userRoles)
            };
        }

        private async Task<ExternalAuthUser> VerifyGoogleAsync(ExternalRequest request)
        {
            var token = !string.IsNullOrWhiteSpace(request.IdToken) ? request.IdToken : request.AccessToken;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new BadRequestException("Google IdToken or AccessToken is required");
            }

            // 1. Try Google JWT ID Token validation
            var payload = await VerifyGoogleToken(token);
            if (payload != null && !string.IsNullOrWhiteSpace(payload.Email))
            {
                return new ExternalAuthUser
                {
                    Provider = "Google",
                    ProviderKey = payload.Subject,
                    Email = payload.Email,
                    FirstName = payload.GivenName ?? payload.Name ?? string.Empty,
                    LastName = payload.FamilyName ?? string.Empty
                };
            }

            // 2. Try Google UserInfo endpoint with token
            try
            {
                var client = _httpClientFactory.CreateClient();
                using var msg = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
                msg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await client.SendAsync(msg);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    var sub = root.TryGetProperty("sub", out var subProp) ? subProp.GetString() : null;
                    var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
                    var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : string.Empty;
                    var givenName = root.TryGetProperty("given_name", out var gProp) ? gProp.GetString() : name;
                    var familyName = root.TryGetProperty("family_name", out var fProp) ? fProp.GetString() : string.Empty;

                    if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(sub))
                    {
                        return new ExternalAuthUser
                        {
                            Provider = "Google",
                            ProviderKey = sub,
                            Email = email,
                            FirstName = givenName ?? string.Empty,
                            LastName = familyName ?? string.Empty
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to verify Google token via userinfo endpoint");
            }

            throw new BadRequestException("Invalid Google Token or Email not provided");
        }

        private async Task<ExternalAuthUser> VerifyGithubAsync(ExternalRequest request)
        {
            var token = !string.IsNullOrWhiteSpace(request.AccessToken) ? request.AccessToken : request.IdToken;

            // If Code is passed or token looks like an authorization code, exchange it
            if (string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(request.Code))
            {
                token = await ExchangeGithubCodeForTokenAsync(request.Code);
            }
            else if (!string.IsNullOrWhiteSpace(token) && token.Length < 30 && !token.StartsWith("gh", StringComparison.OrdinalIgnoreCase))
            {
                var exchanged = await ExchangeGithubCodeForTokenAsync(token);
                if (!string.IsNullOrWhiteSpace(exchanged))
                {
                    token = exchanged;
                }
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new BadRequestException("GitHub AccessToken or Code is required");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Tech-Education-App");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userResponse = await client.GetAsync("https://api.github.com/user");
            if (!userResponse.IsSuccessStatusCode)
            {
                throw new BadRequestException("Invalid GitHub token or authorization failed");
            }

            var userJson = await userResponse.Content.ReadAsStringAsync();
            using var userDoc = System.Text.Json.JsonDocument.Parse(userJson);
            var root = userDoc.RootElement;

            var id = root.TryGetProperty("id", out var idProp) ? idProp.GetInt64().ToString() : null;
            var login = root.TryGetProperty("login", out var loginProp) ? loginProp.GetString() : null;
            var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : login;
            var email = root.TryGetProperty("email", out var emailProp) && emailProp.ValueKind == System.Text.Json.JsonValueKind.String ? emailProp.GetString() : null;

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BadRequestException("Could not retrieve GitHub user profile");
            }

            // If email is private on GitHub profile, retrieve from /user/emails
            if (string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    var emailsResponse = await client.GetAsync("https://api.github.com/user/emails");
                    if (emailsResponse.IsSuccessStatusCode)
                    {
                        var emailsJson = await emailsResponse.Content.ReadAsStringAsync();
                        using var emailsDoc = System.Text.Json.JsonDocument.Parse(emailsJson);
                        if (emailsDoc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var item in emailsDoc.RootElement.EnumerateArray())
                            {
                                var isPrimary = item.TryGetProperty("primary", out var p) && p.GetBoolean();
                                var isVerified = item.TryGetProperty("verified", out var v) && v.GetBoolean();
                                var itemEmail = item.TryGetProperty("email", out var e) ? e.GetString() : null;

                                if (isPrimary && !string.IsNullOrWhiteSpace(itemEmail))
                                {
                                    email = itemEmail;
                                    break;
                                }
                                if (isVerified && string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(itemEmail))
                                {
                                    email = itemEmail;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to retrieve GitHub user emails");
                }
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                email = $"{login ?? id}@users.noreply.github.com";
            }

            var firstName = string.Empty;
            var lastName = string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
            {
                var parts = name.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                firstName = parts[0];
                lastName = parts.Length > 1 ? parts[1] : string.Empty;
            }
            else
            {
                firstName = login ?? "GitHubUser";
            }

            return new ExternalAuthUser
            {
                Provider = "GitHub",
                ProviderKey = id,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
        }

        private async Task<string?> ExchangeGithubCodeForTokenAsync(string code)
        {
            try
            {
                var clientId = _configuration["Authentication:Github:ClientId"];
                var clientSecret = _configuration["Authentication:Github:ClientSecret"];
                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                    return null;

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["code"] = code
                });

                var response = await client.PostAsync("https://github.com/login/oauth/access_token", content);
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("access_token", out var tokenProp))
                {
                    return tokenProp.GetString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to exchange GitHub authorization code for token");
            }
            return null;
        }

        private async Task<string> GenerateUniqueUsernameAsync(string baseName)
        {
            var clean = Regex.Replace(baseName, @"[^a-zA-Z0-9_]", "").Trim();
            if (string.IsNullOrEmpty(clean)) clean = "user";
            var candidate = clean;
            var exists = await _repository.GetUserByNameAsync(candidate);
            if (exists == null) return candidate;

            for (int i = 0; i < 5; i++)
            {
                candidate = $"{clean}_{Guid.NewGuid().ToString("N")[..5]}";
                exists = await _repository.GetUserByNameAsync(candidate);
                if (exists == null) return candidate;
            }
            return $"{clean}_{Guid.NewGuid():N}";
        }

        public async Task<LoginResponse> RefreshAsync(string refreshToken)
        {
            var rotated = await _tokenService.RotateRefreshTokenAsync(refreshToken);
            if (rotated is null)
            {
                throw new UnauthorizeException("Invalid or expired refresh token");
            }

            var (user, newRefreshToken) = rotated.Value;

            var userRoles = await _userRoleRepository.GetUserRolesAsync(user.Id);
            var roles = userRoles.Select(x => x.Name).ToList();

            return new LoginResponse
            {
                User = _mapper.Map<UserResponse>(user),
                Token = _tokenService.CreateAccessToken(user, roles!),
                RefreshToken = newRefreshToken,
                Roles = _mapper.Map<List<RoleResponse>>(userRoles)
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await _tokenService.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task LogoutAllAsync(string userId)
        {
            await _tokenService.RevokeAllRefreshTokensAsync(userId);
        }

        private async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _configuration["Authentication:Google:ClientId"]! }
                };
                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Google token validation failed");
                return null;
            }
        }

        private class ExternalAuthUser
        {
            public string Provider { get; set; } = string.Empty;
            public string ProviderKey { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
        }

}