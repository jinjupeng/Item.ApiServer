using ApiServer.Application.DTOs.Auth;
using ApiServer.Application.DTOs.Permission;
using ApiServer.Application.DTOs.Role;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Domain.Entities;
using ApiServer.Domain.Enums;
using ApiServer.Shared.Common;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiServer.Application.Services
{
    /// <summary>
    /// 认证服务实现
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IAuthSecurityService _authSecurityService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IUserRepository userRepository,
            IMenuRepository menuRepository,
            IAuthSecurityService authSecurityService,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _menuRepository = menuRepository;
            _authSecurityService = authSecurityService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public async Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(dto.CaptchaKey) || !string.IsNullOrWhiteSpace(dto.CaptchaCode))
                {
                    var captchaValid = await _authSecurityService.ValidateCaptchaAsync(
                        dto.CaptchaKey ?? string.Empty,
                        dto.CaptchaCode ?? string.Empty,
                        consume: true);

                    if (!captchaValid)
                    {
                        return ApiResult<LoginResponseDto>.Failed("验证码错误或已过期");
                    }
                }

                // 验证用户名和密码
                var user = await _userRepository.GetByUsernameAsync(dto.Username);
                if (user == null)
                {
                    return ApiResult<LoginResponseDto>.Failed("用户名或密码错误");
                }
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                {
                    return ApiResult<LoginResponseDto>.Failed("用户名或密码错误");
                }

                if (user.Status != UserStatus.Enabled)
                {
                    return ApiResult<LoginResponseDto>.Failed("用户账号已被禁用");
                }

                // 生成JWT令牌（包含角色与权限声明）
                var token = await GenerateJwtToken(user);
                var refreshToken = await _authSecurityService.IssueRefreshTokenAsync(user.Id);

                // 获取用户信息
                var userInfo = await BuildUserInfoAsync(user.Id, user);

                var response = new LoginResponseDto
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresIn = 3600, // 1小时
                    UserInfo = userInfo
                };

                return ApiResult<LoginResponseDto>.Succeed(response, "登录成功");
            }
            catch (Exception ex)
            {
                return ApiResult<LoginResponseDto>.Failed($"登录失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public async Task<ApiResult<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            try
            {
                var userId = await _authSecurityService.RedeemRefreshTokenAsync(dto.RefreshToken);

                if (!userId.HasValue)
                {
                    return ApiResult<LoginResponseDto>.Failed("无效的刷新令牌");
                }

                var user = await _userRepository.GetByIdAsync(userId.Value);
                if (user == null)
                {
                    return ApiResult<LoginResponseDto>.Failed("用户不存在");
                }

                // 生成新的令牌（包含角色与权限声明）
                var newToken = await GenerateJwtToken(user);
                var newRefreshToken = await _authSecurityService.IssueRefreshTokenAsync(user.Id);
                var userInfo = await BuildUserInfoAsync(user.Id, user);

                var response = new LoginResponseDto
                {
                    AccessToken = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresIn = 3600,
                    UserInfo = userInfo
                };

                return ApiResult<LoginResponseDto>.Succeed(response, "令牌刷新成功");
            }
            catch (Exception ex)
            {
                return ApiResult<LoginResponseDto>.Failed($"刷新令牌失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        public async Task<ApiResult> LogoutAsync()
        {
            try
            {
                // 这里可以实现令牌黑名单等逻辑
                return ApiResult.Succeed("登出成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"登出失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public async Task<ApiResult> ChangePasswordAsync(long userId, ChangePasswordDto dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return ApiResult.Failed("新密码和确认密码不一致");
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ApiResult.Failed("用户不存在");
                }

                if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
                {
                    return ApiResult.Failed("原密码错误");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("密码修改成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"修改密码失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public async Task<ApiResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.VerificationCode))
                {
                    return ApiResult.Failed("验证码不能为空");
                }

                var codeValid = await _authSecurityService.ValidatePasswordResetCodeAsync(
                    dto.UsernameOrEmail,
                    dto.VerificationCode,
                    consume: true);

                if (!codeValid)
                {
                    return ApiResult.Failed("验证码错误或已过期");
                }

                var user = await _userRepository.GetByUsernameAsync(dto.UsernameOrEmail);
                if (user == null && !string.IsNullOrEmpty(dto.UsernameOrEmail))
                {
                    user = await _userRepository.GetByEmailAsync(dto.UsernameOrEmail);
                }

                if (user == null)
                {
                    return ApiResult.Failed("用户不存在");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("密码重置成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"重置密码失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        public async Task<ApiResult<UserInfoDto>> GetCurrentUserInfoAsync(long userId)
        {
            try
            {
                var user = await _userRepository.GetUserWithRolesAsync(userId);
                if (user == null)
                {
                    return ApiResult<UserInfoDto>.Failed("用户不存在");
                }

                var userInfo = new UserInfoDto
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    NickName = user.NickName ?? user.Name,
                    Avatar = user.Portrait,
                    Email = user.Email,
                    Phone = user.Phone,
                    OrgId = user.OrgId,
                    OrgName = user.Organization?.Name,
                    Roles = user.UserRoles.Select(ur => new BaseRoleDto
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name,
                        Code = ur.Role.Code ?? ""
                    }).ToList()
                };

                return ApiResult<UserInfoDto>.Succeed(userInfo);
            }
            catch (Exception ex)
            {
                return ApiResult<UserInfoDto>.Failed($"获取用户信息失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取用户权限详情
        /// </summary>
        public async Task<ApiResult<UserPermissionDto>> GetUserPermissionsAsync(long userId)
        {
            try
            {
                var menus = await _menuRepository.GetMenusByUserIdAsync(userId);
                var apis = await _userRepository.GetUserPermissionsAsync(userId);

                var menuPermissions = menus.Select(m => new MenuPermissionDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    Icon = m.Icon,
                    Path = m.Url,
                    ParentId = m.ParentId,
                    Sort = m.Sort
                }).ToList();

                var permission = new UserPermissionDto
                {
                    UserId = userId,
                    Menus = BuildMenuPermissionTree(menuPermissions),
                    Apis = apis.Select(a => a.Url ?? "").Where(url => !string.IsNullOrEmpty(url)).ToList()
                };

                return ApiResult<UserPermissionDto>.Succeed(permission);
            }
            catch (Exception ex)
            {
                return ApiResult<UserPermissionDto>.Failed($"获取用户权限失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取用户权限代码列表
        /// </summary>
        public async Task<ApiResult<List<string>>> GetUserPermissionListAsync(long userId)
        {
            try
            {
                var user = await _userRepository.GetUserWithRolesAsync(userId);
                if (user == null)
                {
                    return ApiResult<List<string>>.Failed("用户不存在");
                }

                var permissionCodes = new List<string>();

                // 获取用户的所有角色的菜单权限
                foreach (var userRole in user.UserRoles)
                {
                    var roleMenus = await _menuRepository.GetMenusByRoleIdAsync(userRole.RoleId);
                    foreach (var menu in roleMenus)
                    {
                        if (!string.IsNullOrEmpty(menu.Code))
                        {
                            permissionCodes.Add(menu.Code);
                        }
                    }
                }

                // 获取用户的API/按钮权限
                var userApis = await _userRepository.GetUserPermissionsAsync(userId);
                foreach (var api in userApis)
                {
                    if (!string.IsNullOrEmpty(api.Code))
                    {
                        permissionCodes.Add(api.Code);
                    }
                }

                // 去重
                var uniquePermissions = permissionCodes.Distinct().ToList();
                
                return ApiResult<List<string>>.Succeed(uniquePermissions);
            }
            catch (Exception ex)
            {
                return ApiResult<List<string>>.Failed($"获取用户权限列表失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        public async Task<ApiResult<bool>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetTokenValidationParameters();

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return ApiResult<bool>.Succeed(true);
            }
            catch
            {
                return ApiResult<bool>.Succeed(false);
            }
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        public async Task<ApiResult<CaptchaDto>> GenerateCaptchaAsync()
        {
            try
            {
                var generated = await _authSecurityService.GenerateCaptchaAsync();
                return ApiResult<CaptchaDto>.Succeed(new CaptchaDto
                {
                    Key = generated.Key,
                    Image = generated.ImageDataUrl
                });
            }
            catch (Exception ex)
            {
                return ApiResult<CaptchaDto>.Failed($"生成验证码失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        public async Task<ApiResult<bool>> ValidateCaptchaAsync(string key, string code)
        {
            try
            {
                var isValid = await _authSecurityService.ValidateCaptchaAsync(key, code, consume: false);
                return ApiResult<bool>.Succeed(isValid);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failed($"验证验证码失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 发送重置密码验证码
        /// </summary>
        public async Task<ApiResult> SendResetPasswordCodeAsync(string usernameOrEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usernameOrEmail))
                {
                    return ApiResult.Failed("用户名或邮箱不能为空");
                }

                var user = await _userRepository.GetByUsernameAsync(usernameOrEmail);
                if (user == null)
                {
                    user = await _userRepository.GetByEmailAsync(usernameOrEmail);
                }

                if (user == null)
                {
                    return ApiResult.Succeed("如果用户存在，验证码已发送");
                }

                var code = await _authSecurityService.GeneratePasswordResetCodeAsync(usernameOrEmail);
                var message = _authSecurityService.IsDevelopment
                    ? $"验证码已发送，开发环境验证码：{code}"
                    : "验证码已发送，请查收";

                return ApiResult.Succeed(message);
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"发送验证码失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 生成JWT令牌（包含角色与权限声明）
        /// </summary>
        private async Task<string> GenerateJwtToken(Domain.Entities.User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Username", user.Name),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // 角色声明
            var userWithRoles = await _userRepository.GetUserWithRolesAsync(user.Id);
            if (userWithRoles != null)
            {
                foreach (var roleName in userWithRoles.UserRoles.Select(ur => ur.Role.Name))
                {
                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleName));
                    }
                }
            }

            // 权限声明（API/按钮权限 的编码，逗号分隔）
            var permissionCodes = new List<string>();
            var userPermissions = await _userRepository.GetUserPermissionsAsync(user.Id);
            permissionCodes.AddRange(userPermissions.Select(a => a.Code).Where(c => !string.IsNullOrWhiteSpace(c))!);
            var uniquePermissions = permissionCodes.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (uniquePermissions.Count != 0)
            {
                var permissionsValue = string.Join(',', uniquePermissions);
                claims.Add(new Claim("permissions", permissionsValue));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 获取令牌验证参数
        /// </summary>
        private TokenValidationParameters GetTokenValidationParameters()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        private async Task<UserInfoDto> BuildUserInfoAsync(long userId, Domain.Entities.User? user = null)
        {
            var userWithRoles = await _userRepository.GetUserWithRolesAsync(userId);
            var source = userWithRoles ?? user;

            if (source == null)
            {
                return new UserInfoDto();
            }

            return new UserInfoDto
            {
                UserId = source.Id,
                UserName = source.Name,
                NickName = source.NickName ?? source.Name,
                Avatar = source.Portrait,
                Email = source.Email,
                Phone = source.Phone,
                OrgId = source.OrgId,
                OrgName = source.Organization?.Name,
                Roles = source.UserRoles.Select(ur => new BaseRoleDto
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Code = ur.Role.Code ?? string.Empty
                }).ToList()
            };
        }

        /// <summary>
        /// 构建菜单权限树
        /// </summary>
        private List<MenuPermissionDto> BuildMenuPermissionTree(List<MenuPermissionDto> menus)
        {
            var menuDict = menus.ToDictionary(m => m.Id, m => m);
            var rootMenus = new List<MenuPermissionDto>();

            foreach (var menu in menuDict.Values)
            {
                if (menu.ParentId.HasValue && menuDict.ContainsKey(menu.ParentId.Value))
                {
                    menuDict[menu.ParentId.Value].Children.Add(menu);
                }
                else
                {
                    rootMenus.Add(menu);
                }
            }

            return rootMenus.OrderBy(m => m.Sort).ToList();
        }
    }
}
