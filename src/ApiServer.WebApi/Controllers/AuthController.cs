using ApiServer.Application.DTOs.Auth;
using ApiServer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    /// <summary>
    /// 认证控制器
    /// </summary>
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="dto">登录信息</param>
        /// <returns>登录结果</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="dto">刷新令牌信息</param>
        /// <returns>新的令牌</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns>登出结果</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.LogoutAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns>验证码</returns>
        [HttpGet("captcha")]
        public async Task<IActionResult> GenerateCaptcha()
        {
            var result = await _authService.GenerateCaptchaAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="key">验证码Key</param>
        /// <param name="code">验证码</param>
        /// <returns>验证结果</returns>
        [HttpPost("captcha/validate")]
        public async Task<IActionResult> ValidateCaptcha([FromQuery] string key, [FromQuery] string code)
        {
            var result = await _authService.ValidateCaptchaAsync(key, code);
            return HandleResult(result);
        }

        /// <summary>
        /// 发送重置密码验证码
        /// </summary>
        /// <param name="usernameOrEmail">用户名或邮箱</param>
        /// <returns>发送结果</returns>
        [HttpPost("send-reset-code")]
        public async Task<IActionResult> SendResetPasswordCode([FromBody] string usernameOrEmail)
        {
            var result = await _authService.SendResetPasswordCodeAsync(usernameOrEmail);
            return HandleResult(result);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="dto">重置密码信息</param>
        /// <returns>重置结果</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dto">修改密码信息</param>
        /// <returns>修改结果</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(userId.Value, dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        [HttpGet("user-info")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var result = await _authService.GetCurrentUserInfoAsync(userId.Value);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <returns>用户权限</returns>
        [HttpGet("permissions")]
        [Authorize]
        public async Task<IActionResult> GetUserPermissions()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var result = await _authService.GetUserPermissionsAsync(userId.Value);
            return HandleResult(result);
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>验证结果</returns>
        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            var result = await _authService.ValidateTokenAsync(token);
            return HandleResult(result);
        }
    }
}
