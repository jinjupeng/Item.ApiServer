namespace ApiServer.Application.DTOs.Auth
{
    /// <summary>
    /// 登录请求DTO
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 验证码
        /// </summary>
        public string? CaptchaCode { get; set; }

        /// <summary>
        /// 验证码Key
        /// </summary>
        public string? CaptchaKey { get; set; }

        /// <summary>
        /// 记住我
        /// </summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// 登录响应DTO
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// 令牌类型
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfoDto UserInfo { get; set; } = new();
    }

    /// <summary>
    /// 用户信息DTO
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string? OrgName { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<string> Permissions { get; set; } = new();
    }

    /// <summary>
    /// 刷新令牌DTO
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// 修改密码DTO
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// 重置密码DTO
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// 用户名或邮箱
        /// </summary>
        public string UsernameOrEmail { get; set; } = string.Empty;

        /// <summary>
        /// 验证码
        /// </summary>
        public string VerificationCode { get; set; } = string.Empty;

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// 用户权限DTO
    /// </summary>
    public class UserPermissionDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 菜单权限
        /// </summary>
        public List<MenuPermissionDto> Menus { get; set; } = new();

        /// <summary>
        /// API权限
        /// </summary>
        public List<string> Apis { get; set; } = new();
    }

    /// <summary>
    /// 菜单权限DTO
    /// </summary>
    public class MenuPermissionDto
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 菜单编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 组件
        /// </summary>
        public string? Component { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<MenuPermissionDto> Children { get; set; } = new();
    }
}
