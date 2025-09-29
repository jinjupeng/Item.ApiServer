using ApiServer.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Domain.Entities
{

    /// <summary>
    /// 系统审计日志实体
    /// </summary>
    public class AuditLog : BaseEntity
    {
        /// <summary>
        /// 操作类型 (Create, Update, Delete, Login, Logout, etc.)
        /// </summary>
        [StringLength(32)]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 操作模块 (User, Config, Application, etc.)
        /// </summary>
        [StringLength(32)]
        public string Module { get; set; } = string.Empty;

        /// <summary>
        /// 操作描述
        /// </summary>
        [StringLength(128)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 操作前数据 (JSON格式)
        /// </summary>
        public string? OldData { get; set; }

        /// <summary>
        /// 操作后数据 (JSON格式)
        /// </summary>
        public string? NewData { get; set; }

        /// <summary>
        /// 操作结果 (Success, Failed)
        /// </summary>
        [StringLength(32)]
        public string Result { get; set; } = string.Empty;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 操作用户ID
        /// </summary>
        [StringLength(32)]
        public long? UserId { get; set; }

        /// <summary>
        /// 操作用户名
        /// </summary>
        [StringLength(32)]
        public string? UserName { get; set; }

        /// <summary>
        /// 用户IP地址
        /// </summary>
        [StringLength(32)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        [StringLength(128)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        [StringLength(256)]
        public string? RequestPath { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        [StringLength(32)]
        public string? RequestMethod { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string? RequestData { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        public int? ResponseStatusCode { get; set; }

        /// <summary>
        /// 操作耗时(毫秒)
        /// </summary>
        public long? Duration { get; set; }

        /// <summary>
        /// 关联实体ID
        /// </summary>
        [StringLength(32)]
        public string? EntityId { get; set; }

        /// <summary>
        /// 关联实体类型
        /// </summary>
        [StringLength(32)]
        public string? EntityType { get; set; }
    }
}
