using System.Collections.Generic;

namespace ApiServer.Model.Model
{
    public class JwtProperties
    {
        /// <summary>
        /// 是否开启JWT，即注入相关的类对象
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// JWT密钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// JWT有效时间
        /// </summary>
        public long Expiration { get; set; }

        /// <summary>
        /// 前端向后端传递JWT时使用HTTP的header名称
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 允许哪些域对本服务的跨域请求
        /// </summary>
        public List<string> CorsAllowedOrigins { get; set; }

        /// <summary>
        /// 允许哪些HTTP方法跨域
        /// </summary>
        public List<string> CorsAllowedMethods { get; set; }

        /// <summary>
        /// 用户获取JWT令牌发送的用户名参数名称
        /// </summary>
        public string UserParamName { get; set; } = "username";

        /// <summary>
        /// 用户获取JWT令牌发送的密码参数名称
        /// </summary>
        public string PwdParamName { get; set; } = "password";

        /// <summary>
        /// 是否关闭csrf跨站攻击防御功能
        /// </summary>
        public bool CsrfDisabled { get; set; } = true;

        /// <summary>
        /// 是否使用默认的JWTAuthController
        /// </summary>
        public bool UseDefaultController { get; set; } = true;

        /// <summary>
        /// 开发过程临时开放的URI
        /// </summary>
        public List<string> DevOPeningURI { get; set; }

        /// <summary>
        /// 权限全面开放的接口，不需要JWT令牌就可以访问
        /// </summary>
        public List<string> PermitAllURI { get; set; }
    }
}
