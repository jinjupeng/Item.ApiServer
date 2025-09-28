using ApiServer.Application.DTOs.Audit;
using ApiServer.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 系统审计日志服务接口
    /// </summary>
    public interface ISystemAuditLogService
    {
        /// <summary>
        /// 记录系统审计日志
        /// </summary>
        Task LogAsync(SystemAuditLogDto auditLog);

        /// <summary>
        /// 记录操作审计日志
        /// </summary>
        Task LogActionAsync(string action, string module, string description,
            string? userId = null, string? userName = null, string? ipAddress = null,
            string? userAgent = null, string? requestPath = null, string? requestMethod = null,
            string? requestData = null, string? oldData = null, string? newData = null,
            string? entityId = null, string? entityType = null, int? responseStatusCode = null,
            long? duration = null, string? errorMessage = null);

        /// <summary>
        /// 获取系统审计日志分页列表
        /// </summary>
        Task<ApiResult<PagedResult<SystemAuditLogDto>>> GetAuditLogsAsync(SystemAuditLogQueryDto parameters);

        /// <summary>
        /// 根据ID获取审计日志详情
        /// </summary>
        Task<ApiResult<SystemAuditLogDto>> GetAuditLogByIdAsync(string id);

        /// <summary>
        /// 获取用户操作日志
        /// </summary>
        Task<ApiResult<PagedResult<SystemAuditLogDto>>> GetUserAuditLogsAsync(string userId, SystemAuditLogQueryDto parameters);

        /// <summary>
        /// 获取模块操作日志
        /// </summary>
        Task<ApiResult<PagedResult<SystemAuditLogDto>>> GetModuleAuditLogsAsync(string module, SystemAuditLogQueryDto parameters);

        /// <summary>
        /// 导出审计日志
        /// </summary>
        Task<ApiResult<string>> ExportAuditLogsAsync(SystemAuditLogExportDto parameters);

        /// <summary>
        /// 清理过期审计日志
        /// </summary>
        Task<ApiResult<int>> CleanupExpiredLogsAsync(int retentionDays);

        /// <summary>
        /// 获取审计日志统计信息
        /// </summary>
        Task<ApiResult<AuditLogStatisticsDto>> GetAuditLogStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }

}
