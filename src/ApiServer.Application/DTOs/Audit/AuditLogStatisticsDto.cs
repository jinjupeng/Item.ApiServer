using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Application.DTOs.Audit
{
    /// <summary>
    /// 审计日志统计信息DTO
    /// </summary>
    public class AuditLogStatisticsDto
    {
        /// <summary>
        /// 总操作数
        /// </summary>
        public int TotalOperations { get; set; }

        /// <summary>
        /// 成功操作数
        /// </summary>
        public int SuccessOperations { get; set; }

        /// <summary>
        /// 失败操作数
        /// </summary>
        public int FailedOperations { get; set; }

        /// <summary>
        /// 按操作类型统计
        /// </summary>
        public Dictionary<string, int> ActionStatistics { get; set; } = new();

        /// <summary>
        /// 按模块统计
        /// </summary>
        public Dictionary<string, int> ModuleStatistics { get; set; } = new();

        /// <summary>
        /// 按用户统计
        /// </summary>
        public Dictionary<string, int> UserStatistics { get; set; } = new();

        /// <summary>
        /// 按日期统计
        /// </summary>
        public Dictionary<string, int> DateStatistics { get; set; } = new();
    }
}
