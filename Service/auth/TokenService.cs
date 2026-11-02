using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using WepApi.Models;
using WepApi.Repository;
using WepApi.Service.Interface;

namespace WepApi.Service;

public class TokenService : ITokenService
{
    private const string RefreshProvider = "RefreshProvider";
    private readonly IConfiguration _configuration;
    private readonly IUserTokenRepository _tokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TokenService> _logger;
    
    public TokenService(
        IConfiguration configuration,
        IUserTokenRepository tokenRepository,
        IUserRepository userRepository,
        ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _logger = logger;
    }
    
     public string CreateAccessToken(AspNetUsers user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Email, user.Email ?? ""),
                new("FullName", $"{user.FirstName} {user.LastName}".Trim())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role ?? ""));
            }

            if (user.IsSuperUser)
            {
                claims.Add(new Claim("IsSuperUser", "true"));
                if (!roles.Contains("SuperAdmin"))
                {
                    claims.Add(new Claim(ClaimTypes.Role, "SuperAdmin"));
                }
            }

            var secret = _configuration["JWT:Secret"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var minutes = _configuration.GetValue<int?>("JWT:AccessTokenMinutes") ?? 60;

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> IssueRefreshTokenAsync(string userId)
        {
            var days = _configuration.GetValue<int?>("JWT:RefreshTokenDays") ?? 14;

            // The id names the row; the secret is the part that must be proven.
            var tokenId = Guid.NewGuid().ToString("N");
            var secret = Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

            var stored = new StoredRefreshToken
            {
                Hash = Hash(secret),
                ExpiresUtc = DateTime.UtcNow.AddDays(days)
            };

            await _tokenRepository.AddAsync(new AspNetUserTokens
            {
                UserId = userId,
                LoginProvider = RefreshProvider,
                Name = tokenId,
                Value = JsonSerializer.Serialize(stored)
            });
            await _tokenRepository.SaveChangesAsync();

            return $"{tokenId}.{secret}";
        }

        public async Task<(AspNetUsers User, string RefreshToken)?> RotateRefreshTokenAsync(string refreshToken)
        {
            var parsed = Parse(refreshToken);
            if (parsed is null)
            {
                return null;
            }

            var (tokenId, secret) = parsed.Value;

            var row = await _tokenRepository.GetByProviderAndNameAsync(RefreshProvider, tokenId);
            if (row is null)
            {
                _logger.LogWarning("Refresh rejected: token id {TokenId} is not active.", tokenId);
                return null;
            }

            var stored = Deserialize(row.Value);
            if (stored is null)
            {
                _tokenRepository.Delete(row);
                await _tokenRepository.SaveChangesAsync();
                _logger.LogError("Refresh rejected: stored token {TokenId} is unreadable; discarded.", tokenId);
                return null;
            }

            // Constant-time compare so a wrong secret cannot be found by timing.
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(Hash(secret)),
                    Encoding.UTF8.GetBytes(stored.Hash)))
            {
                // Correct id, wrong secret: the id leaked. Drop every refresh token
                // this user holds and make them sign in again.
                _logger.LogWarning(
                    "Refresh rejected: bad secret for token {TokenId}; revoking all sessions for user {UserId}.",
                    tokenId, row.UserId);
                await RevokeAllRefreshTokensAsync(row.UserId);
                return null;
            }

            if (stored.ExpiresUtc <= DateTime.UtcNow)
            {
                _tokenRepository.Delete(row);
                await _tokenRepository.SaveChangesAsync();
                return null;
            }

            var user = await _userRepository.GetUserByIdAsync(row.UserId);
            if (user is null || IsLockedOut(user))
            {
                _tokenRepository.Delete(row);
                await _tokenRepository.SaveChangesAsync();
                return null;
            }

            // Rotate: consume the presented token before issuing its replacement.
            _tokenRepository.Delete(row);
            await _tokenRepository.SaveChangesAsync();

            var replacement = await IssueRefreshTokenAsync(user.Id);
            return (user, replacement);
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var parsed = Parse(refreshToken);
            if (parsed is null)
            {
                return;
            }

            var row = await _tokenRepository.GetByProviderAndNameAsync(RefreshProvider, parsed.Value.TokenId);
            if (row is not null)
            {
                _tokenRepository.Delete(row);
                await _tokenRepository.SaveChangesAsync();
            }
        }

        public async Task RevokeAllRefreshTokensAsync(string userId)
        {
            var rows = await _tokenRepository.GetByUserAndProviderAsync(userId, RefreshProvider);
            if (rows.Count > 0)
            {
                _tokenRepository.DeleteRange(rows);
                await _tokenRepository.SaveChangesAsync();
            }
        }

        private static bool IsLockedOut(AspNetUsers user) =>
            string.Equals(user.Status, "Inactive", StringComparison.OrdinalIgnoreCase)
            || (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow);

        private static (string TokenId, string Secret)? Parse(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            var separator = refreshToken.IndexOf('.');
            if (separator <= 0 || separator == refreshToken.Length - 1)
            {
                return null;
            }

            return (refreshToken[..separator], refreshToken[(separator + 1)..]);
        }

        private static string Hash(string value) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

        private static string Base64UrlEncode(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        private static StoredRefreshToken? Deserialize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<StoredRefreshToken>(value);
            }
            catch (JsonException)
            {
                return null;
            }
        }
        private sealed class StoredRefreshToken
        {
            public string Hash { get; set; } = string.Empty;
            public DateTime ExpiresUtc { get; set; }
        }
    
    
}