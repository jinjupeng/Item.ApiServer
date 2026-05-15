namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 认证安全辅助服务
    /// </summary>
    public interface IAuthSecurityService
    {
        /// <summary>
        /// 是否为开发环境
        /// </summary>
        bool IsDevelopment { get; }

        /// <summary>
        /// 生成验证码
        /// </summary>
        Task<(string Key, string Code, string ImageDataUrl)> GenerateCaptchaAsync();

        /// <summary>
        /// 校验验证码
        /// </summary>
        Task<bool> ValidateCaptchaAsync(string key, string code, bool consume);

        /// <summary>
        /// 生成密码重置验证码
        /// </summary>
        Task<string> GeneratePasswordResetCodeAsync(string usernameOrEmail);

        /// <summary>
        /// 校验密码重置验证码
        /// </summary>
        Task<bool> ValidatePasswordResetCodeAsync(string usernameOrEmail, string code, bool consume);

        /// <summary>
        /// 生成刷新令牌
        /// </summary>
        Task<string> IssueRefreshTokenAsync(long userId);

        /// <summary>
        /// 校验并消费刷新令牌
        /// </summary>
        Task<long?> RedeemRefreshTokenAsync(string refreshToken);
    }
}
