using ApiServer.Application.DTOs.Audit;
using ApiServer.Shared.Common;

namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 系统审计日志服务接口
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// 记录系统审计日志
        /// </summary>
        Task LogAsync(AuditLogDto auditLog);

        /// <summary>
        /// 记录操作审计日志
        /// </summary>
        Task LogActionAsync(string action, string module, string description,
            long? userId = null, string? userName = null, string? ipAddress = null,
            string? userAgent = null, string? requestPath = null, string? requestMethod = null,
            string? requestData = null, string? oldData = null, string? newData = null,
            string? entityId = null, string? entityType = null, int? responseStatusCode = null,
            long? duration = null, string? errorMessage = null);

        /// <summary>
        /// 获取系统审计日志分页列表
        /// </summary>
        Task<ApiResult<PagedResult<AuditLogDto>>> GetAuditLogsAsync(AuditLogQueryDto parameters);

        /// <summary>
        /// 根据ID获取审计日志详情
        /// </summary>
        Task<ApiResult<AuditLogDto>> GetAuditLogByIdAsync(long id);

        /// <summary>
        /// 获取用户操作日志
        /// </summary>
        Task<ApiResult<PagedResult<AuditLogDto>>> GetUserAuditLogsAsync(long userId, AuditLogQueryDto parameters);

        /// <summary>
        /// 获取模块操作日志
        /// </summary>
        Task<ApiResult<PagedResult<AuditLogDto>>> GetModuleAuditLogsAsync(string module, AuditLogQueryDto parameters);

        /// <summary>
        /// 导出审计日志
        /// </summary>
        Task<ApiResult<string>> ExportAuditLogsAsync(AuditLogExportDto parameters);

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
