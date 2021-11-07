using Common.Utility.JWT;

namespace ApiServer.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiServerOptions
    {
        public JwtOptions JwtOptions { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnString { get; set; }
    }
}
