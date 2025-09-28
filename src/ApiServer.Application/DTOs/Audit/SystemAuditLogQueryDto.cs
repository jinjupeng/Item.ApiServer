using ApiServer.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Application.DTOs.Audit
{
    /// <summary>
    /// 系统审计日志查询参数
    /// </summary>
    public class SystemAuditLogQueryDto : BaseQueryDto
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// 操作模块
        /// </summary>
        public string? Module { get; set; }

        /// <summary>
        /// 操作用户ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public string? Result { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        public string? EntityType { get; set; }
    }
}
