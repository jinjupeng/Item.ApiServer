using System.ComponentModel;

namespace ApiServer.Common.Cache
{
    /// <summary>
    /// 缓存键
    /// </summary>
    public static class CacheKey
    {
        /// <summary>
        /// 登录验证码
        /// <para>Admin:Auth:VerifyCode:用户id</para>
        /// </summary>
        [Description("登录验证码")]
        public const string VerifyCodeKey = "Admin:Auth:VerifyCode:{0}";

        /// <summary>
        /// 邮箱验证码
        /// <para>Admin:Email:VerifyCode:用户id</para>
        /// </summary>
        [Description("邮箱验证码")]
        public const string EmailVerifyCodeKey = "Admin:Email:VerifyCode:{0}";

        /// <summary>
        /// 手机号验证码
        /// <para>Admin:Phone:VerifyCode:用户id</para>
        /// </summary>
        [Description("手机号验证码")]
        public const string PhoneVerifyCodeKey = "Admin:Phone:VerifyCode:{0}";

        /// <summary>
        /// 刷新令牌 
        /// <para>Admin:Auth:RefreshToken:用户id</para>
        /// </summary>
        [Description("刷新令牌")]
        public const string AUTH_REFRESH_TOKEN = "Admin:Auth:RefreshToken:{0}";

        /// <summary>
        /// 账户信息
        /// <para>Admin:Account:Info:用户id</para>
        /// </summary>
        [Description("账户信息")]
        public const string Account = "Admin:Account:Info:{0}";

        /// <summary>
        /// 用户权限
        /// <para>Admin:Account:Permissions:用户id</para>
        /// </summary>
        [Description("用户权限")]
        public const string UserPermissions = "Admin:Account:Permissions:{0}";
    }
}
