using ApiServer.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Application.DTOs.Audit
{
    /// <summary>
    /// 系统审计日志DTO
    /// </summary>
    public class SystemAuditLogDto
    {
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 操作类型
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 操作模块
        /// </summary>
        public string Module { get; set; } = string.Empty;

        /// <summary>
        /// 操作描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 操作前数据
        /// </summary>
        public string? OldData { get; set; }

        /// <summary>
        /// 操作后数据
        /// </summary>
        public string? NewData { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public string Result { get; set; } = string.Empty;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 操作用户ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 操作用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 用户IP地址
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        public string? RequestPath { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
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
        public string? EntityId { get; set; }

        /// <summary>
        /// 关联实体类型
        /// </summary>
        public string? EntityType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

}
