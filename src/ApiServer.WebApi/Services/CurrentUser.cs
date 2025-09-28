using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApiServer.Application.Interfaces;

namespace ApiServer.WebApi.Services
{
    /// <summary>
    /// 从HTTP上下文与Authorization头中解析当前用户信息
    /// </summary>
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUser> _logger;
        private ClaimsPrincipal? _principal;
        private string? _token;
        private bool _parsed;

        public CurrentUser(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUser> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

        public long? UserId
        {
            get
            {
                var val = GetClaim("UserId") ?? GetClaim(ClaimTypes.NameIdentifier);
                return long.TryParse(val, out var id) ? id : null;
            }
        }

        public string? Username => GetClaim("Username") ?? GetClaim(ClaimTypes.Name);

        public IEnumerable<string> Roles => Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value)
            .Distinct();

        public string? Token
        {
            get
            {
                EnsureParsed();
                return _token;
            }
        }

        public IEnumerable<Claim> Claims => Principal?.Claims ?? Enumerable.Empty<Claim>();

        public string? GetClaim(string type) => Claims.FirstOrDefault(c => c.Type == type)?.Value;

        public bool HasRole(string role) => Roles.Contains(role);

        private ClaimsPrincipal? Principal
        {
            get
            {
                EnsureParsed();
                return _principal;
            }
        }

        private void EnsureParsed()
        {
            if (_parsed) return;
            _parsed = true;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return;

            // 优先使用框架认证后的 User
            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                _principal = httpContext.User;
            }

            // 尝试从Authorization头提取并解析JWT（不进行签名验证，这里仅用于读取Claim）
            if (_principal == null)
            {
                var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    _token = authHeader["Bearer ".Length..].Trim();

                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(_token);
                        var identity = new ClaimsIdentity(jwtToken.Claims, authenticationType: "Jwt");
                        _principal = new ClaimsPrincipal(identity);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogWarning($"无法解析Authorization头中的JWT令牌，错误信息：{ex.Message}");
                    }
                }
            }
        }
    }
}
