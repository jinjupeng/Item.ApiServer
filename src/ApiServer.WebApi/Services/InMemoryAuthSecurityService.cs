using System.Security.Cryptography;
using System.Text;
using ApiServer.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ApiServer.WebApi.Services
{
    /// <summary>
    /// 基于内存缓存的认证安全服务
    /// </summary>
    public class InMemoryAuthSecurityService : IAuthSecurityService
    {
        private static readonly TimeSpan CaptchaTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan ResetCodeTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan RefreshTokenTtl = TimeSpan.FromDays(7);

        private readonly IMemoryCache _memoryCache;
        private readonly IHostEnvironment _hostEnvironment;

        public InMemoryAuthSecurityService(IMemoryCache memoryCache, IHostEnvironment hostEnvironment)
        {
            _memoryCache = memoryCache;
            _hostEnvironment = hostEnvironment;
        }

        public bool IsDevelopment => _hostEnvironment.IsDevelopment();

        public Task<(string Key, string Code, string ImageDataUrl)> GenerateCaptchaAsync()
        {
            var code = RandomNumberGenerator.GetInt32(1000, 9999).ToString();
            var key = Guid.NewGuid().ToString("N");

            _memoryCache.Set(GetCaptchaCacheKey(key), code, CaptchaTtl);

            return Task.FromResult((key, code, BuildCaptchaImage(code)));
        }

        public Task<bool> ValidateCaptchaAsync(string key, string code, bool consume)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code))
            {
                return Task.FromResult(false);
            }

            var cacheKey = GetCaptchaCacheKey(key);
            if (!_memoryCache.TryGetValue<string>(cacheKey, out var expectedCode) ||
                !string.Equals(expectedCode, code, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            if (consume)
            {
                _memoryCache.Remove(cacheKey);
            }

            return Task.FromResult(true);
        }

        public Task<string> GeneratePasswordResetCodeAsync(string usernameOrEmail)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            _memoryCache.Set(GetResetCodeCacheKey(usernameOrEmail), code, ResetCodeTtl);
            return Task.FromResult(code);
        }

        public Task<bool> ValidatePasswordResetCodeAsync(string usernameOrEmail, string code, bool consume)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(code))
            {
                return Task.FromResult(false);
            }

            var cacheKey = GetResetCodeCacheKey(usernameOrEmail);
            if (!_memoryCache.TryGetValue<string>(cacheKey, out var expectedCode) ||
                !string.Equals(expectedCode, code, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            if (consume)
            {
                _memoryCache.Remove(cacheKey);
            }

            return Task.FromResult(true);
        }

        public Task<string> IssueRefreshTokenAsync(long userId)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
            _memoryCache.Set(GetRefreshTokenCacheKey(refreshToken), userId, RefreshTokenTtl);
            return Task.FromResult(refreshToken);
        }

        public Task<long?> RedeemRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Task.FromResult<long?>(null);
            }

            var cacheKey = GetRefreshTokenCacheKey(refreshToken);
            if (!_memoryCache.TryGetValue<long>(cacheKey, out var userId))
            {
                return Task.FromResult<long?>(null);
            }

            _memoryCache.Remove(cacheKey);
            return Task.FromResult<long?>(userId);
        }

        private static string GetCaptchaCacheKey(string key) => $"auth:captcha:{key}";

        private static string GetResetCodeCacheKey(string usernameOrEmail) =>
            $"auth:reset:{NormalizeKey(usernameOrEmail)}";

        private static string GetRefreshTokenCacheKey(string refreshToken) => $"auth:refresh:{refreshToken}";

        private static string NormalizeKey(string value) => value.Trim().ToLowerInvariant();

        private static string BuildCaptchaImage(string code)
        {
            var svg = $"""
                       <svg xmlns="http://www.w3.org/2000/svg" width="120" height="40" viewBox="0 0 120 40">
                         <rect width="120" height="40" fill="#f4f6f8" rx="4" ry="4"/>
                         <line x1="10" y1="8" x2="110" y2="32" stroke="#d0d7de" stroke-width="1"/>
                         <line x1="15" y1="32" x2="105" y2="8" stroke="#d0d7de" stroke-width="1"/>
                         <text x="60" y="27" text-anchor="middle" font-size="24" font-family="Consolas, monospace" fill="#1f2937" letter-spacing="4">{code}</text>
                       </svg>
                       """;

            var bytes = Encoding.UTF8.GetBytes(svg);
            return $"data:image/svg+xml;base64,{Convert.ToBase64String(bytes)}";
        }
    }
}
