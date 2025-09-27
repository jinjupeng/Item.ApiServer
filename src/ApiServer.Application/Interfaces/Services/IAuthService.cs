using ApiServer.Application.DTOs.Auth;
using ApiServer.Shared.Common;

namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        Task<ApiResult<LoginResponseDto>> LoginAsync(LoginDto dto);

        /// <summary>
        /// 刷新令牌
        /// </summary>
        Task<ApiResult<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);

        /// <summary>
        /// 用户登出
        /// </summary>
        Task<ApiResult> LogoutAsync();

        /// <summary>
        /// 修改密码
        /// </summary>
        Task<ApiResult> ChangePasswordAsync(long userId, ChangePasswordDto dto);

        /// <summary>
        /// 重置密码
        /// </summary>
        Task<ApiResult> ResetPasswordAsync(ResetPasswordDto dto);

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        Task<ApiResult<UserInfoDto>> GetCurrentUserInfoAsync(long userId);

        /// <summary>
        /// 获取用户权限
        /// </summary>
        Task<ApiResult<UserPermissionDto>> GetUserPermissionsAsync(long userId);

        /// <summary>
        /// 获取用户权限代码列表
        /// </summary>
        Task<ApiResult<List<string>>> GetUserPermissionListAsync(long userId);

        /// <summary>
        /// 验证令牌
        /// </summary>
        Task<ApiResult<bool>> ValidateTokenAsync(string token);

        /// <summary>
        /// 生成验证码
        /// </summary>
        Task<ApiResult<CaptchaDto>> GenerateCaptchaAsync();

        /// <summary>
        /// 验证验证码
        /// </summary>
        Task<ApiResult<bool>> ValidateCaptchaAsync(string key, string code);

        /// <summary>
        /// 发送重置密码验证码
        /// </summary>
        Task<ApiResult> SendResetPasswordCodeAsync(string usernameOrEmail);
    }

    /// <summary>
    /// 验证码DTO
    /// </summary>
    public class CaptchaDto
    {
        /// <summary>
        /// 验证码Key
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 验证码图片Base64
        /// </summary>
        public string Image { get; set; } = string.Empty;
    }
}
