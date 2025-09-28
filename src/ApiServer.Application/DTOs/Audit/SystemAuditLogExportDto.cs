using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Application.DTOs.Audit
{
    /// <summary>
    /// 系统审计日志导出参数
    /// </summary>
    public class SystemAuditLogExportDto
    {
        public string? SearchTerm { get; set; }
        public string? Action { get; set; }
        public string? Module { get; set; }
        public string? UserId { get; set; }
        public string? Result { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? IpAddress { get; set; }
        public string? EntityType { get; set; }
    }
}
