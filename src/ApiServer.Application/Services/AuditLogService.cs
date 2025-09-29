using ApiServer.Application.DTOs.Audit;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Domain.Entities;
using ApiServer.Shared.Common;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiServer.Application.Services
{
    /// <summary>
    /// 审计日志服务
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogRepository _auditLogRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="auditLogRepository"></param>
        public AuditLogService(IUnitOfWork unitOfWork, IAuditLogRepository auditLogRepository)
        {
            _unitOfWork = unitOfWork;
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogAsync(AuditLogDto auditLogDto)
        {
            var auditLog = auditLogDto.Adapt<AuditLog>();
            await _auditLogRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task LogActionAsync(string action, string module, string description, long? userId = null, string? userName = null, string? ipAddress = null, string? userAgent = null, string? requestPath = null, string? requestMethod = null, string? requestData = null, string? oldData = null, string? newData = null, string? entityId = null, string? entityType = null, int? responseStatusCode = null, long? duration = null, string? errorMessage = null)
        {
            var auditLog = new AuditLog
            {
                Action = action,
                Module = module,
                Description = description,
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                RequestPath = requestPath,
                RequestMethod = requestMethod,
                RequestData = requestData,
                OldData = oldData,
                NewData = newData,
                EntityId = entityId,
                EntityType = entityType,
                ResponseStatusCode = responseStatusCode,
                Duration = duration,
                ErrorMessage = errorMessage,
                Result = string.IsNullOrEmpty(errorMessage) ? "Success" : "Failed",
                CreateTime = DateTime.UtcNow
            };
            await _auditLogRepository.AddAsync(auditLog);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ApiResult<PagedResult<AuditLogDto>>> GetAuditLogsAsync(AuditLogQueryDto parameters)
        {
            var query = _auditLogRepository.GetQueryable();


            // 搜索
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(a =>
                    a.Action.Contains(parameters.SearchTerm) ||
                    a.Module.Contains(parameters.SearchTerm) ||
                    a.Description.Contains(parameters.SearchTerm) ||
                    (a.UserName != null && a.UserName.Contains(parameters.SearchTerm)) ||
                    (a.ErrorMessage != null && a.ErrorMessage.Contains(parameters.SearchTerm)));
            }

            // 过滤
            if (!string.IsNullOrEmpty(parameters.Action))
                query = query.Where(a => a.Action == parameters.Action);

            if (!string.IsNullOrEmpty(parameters.Module))
                query = query.Where(a => a.Module == parameters.Module);

            if (parameters.UserId != null)
                query = query.Where(a => a.UserId == parameters.UserId);

            if (!string.IsNullOrEmpty(parameters.Result))
                query = query.Where(a => a.Result == parameters.Result);

            if (!string.IsNullOrEmpty(parameters.IpAddress))
                query = query.Where(a => a.IpAddress == parameters.IpAddress);

            if (!string.IsNullOrEmpty(parameters.EntityType))
                query = query.Where(a => a.EntityType == parameters.EntityType);

            if (parameters.DateFrom.HasValue)
                query = query.Where(a => a.CreateTime >= parameters.DateFrom.Value);

            if (parameters.DateTo.HasValue)
                query = query.Where(a => a.CreateTime <= parameters.DateTo.Value);

            // 排序
            query = parameters.SortDescending ?
                query.OrderByDescending(a => a.CreateTime) :
                query.OrderBy(a => a.CreateTime);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var dtos = items.Adapt<List<AuditLogDto>>();

            var result = new PagedResult<AuditLogDto>
            {
                Items = dtos,
                Total = totalCount,
                Page = parameters.PageNumber,
                PageSize = parameters.PageSize
            };

            return ApiResult<PagedResult<AuditLogDto>>.Succeed(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResult<AuditLogDto>> GetAuditLogByIdAsync(long id)
        {
            var auditLog = await _auditLogRepository.GetByIdAsync(id);
            if (auditLog == null)
            {
                return ApiResult<AuditLogDto>.Failed("审计日志不存在");
            }

            var dto = auditLog.Adapt<AuditLogDto>();
            return ApiResult<AuditLogDto>.Succeed(dto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<ApiResult<PagedResult<AuditLogDto>>> GetUserAuditLogsAsync(long userId, AuditLogQueryDto parameters)
        {
            parameters.UserId = userId;
            return await GetAuditLogsAsync(parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<ApiResult<PagedResult<AuditLogDto>>> GetModuleAuditLogsAsync(string module, AuditLogQueryDto parameters)
        {
            parameters.Module = module;
            return await GetAuditLogsAsync(parameters);
        }

        /// <summary>
        /// 导出审计日志
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<ApiResult<string>> ExportAuditLogsAsync(AuditLogExportDto parameters)
        {
            try
            {
                var query = _auditLogRepository.GetQueryable();

                // 应用过滤条件
                if (!string.IsNullOrEmpty(parameters.SearchTerm))
                {
                    query = query.Where(a =>
                        a.Action.Contains(parameters.SearchTerm) ||
                        a.Module.Contains(parameters.SearchTerm) ||
                        a.Description.Contains(parameters.SearchTerm) ||
                        (a.UserName != null && a.UserName.Contains(parameters.SearchTerm)));
                }

                if (!string.IsNullOrEmpty(parameters.Action))
                    query = query.Where(a => a.Action == parameters.Action);

                if (!string.IsNullOrEmpty(parameters.Module))
                    query = query.Where(a => a.Module == parameters.Module);

                if (parameters.UserId != null)
                    query = query.Where(a => a.UserId == parameters.UserId);

                if (!string.IsNullOrEmpty(parameters.Result))
                    query = query.Where(a => a.Result == parameters.Result);

                if (!string.IsNullOrEmpty(parameters.IpAddress))
                    query = query.Where(a => a.IpAddress == parameters.IpAddress);

                if (!string.IsNullOrEmpty(parameters.EntityType))
                    query = query.Where(a => a.EntityType == parameters.EntityType);

                if (parameters.DateFrom.HasValue)
                    query = query.Where(a => a.CreateTime >= parameters.DateFrom.Value);

                if (parameters.DateTo.HasValue)
                    query = query.Where(a => a.CreateTime <= parameters.DateTo.Value);

                // 排序
                query = query.OrderByDescending(a => a.CreateTime);

                var auditLogs = await query.ToListAsync();

                // 生成CSV数据
                var csvData = GenerateCsvData(auditLogs);

                return ApiResult<string>.Succeed(csvData);
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Failed($"导出审计日志失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 清理过期审计日志
        /// </summary>
        /// <param name="retentionDays"></param>
        /// <returns></returns>
        public async Task<ApiResult<int>> CleanupExpiredLogsAsync(int retentionDays)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            var logsToDelete = await _auditLogRepository.FindAsync(x => x.CreateTime < cutoffDate);
            
            if (logsToDelete.Any())
            {
                await _auditLogRepository.DeleteRangeAsync(logsToDelete);
                var count = await _unitOfWork.SaveChangesAsync();
                return ApiResult<int>.Succeed(count, $"{count} 条过期日志已经被清理");
            }

            return ApiResult<int>.Succeed(0, "没有需要清理的日志");
        }

        public async Task<ApiResult<AuditLogStatisticsDto>> GetAuditLogStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _auditLogRepository.GetQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(l => l.CreateTime >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(l => l.CreateTime <= endDate.Value);
            }

            var stats = new AuditLogStatisticsDto
            {
                TotalOperations = await query.CountAsync(),
                SuccessOperations = await query.CountAsync(l => l.Result == "Success"),
                FailedOperations = await query.CountAsync(l => l.Result == "Failed"),
                ActionStatistics = await query.GroupBy(l => l.Action).ToDictionaryAsync(g => g.Key, g => g.Count()),
                ModuleStatistics = await query.GroupBy(l => l.Module).ToDictionaryAsync(g => g.Key, g => g.Count()),
                UserStatistics = await query.Where(l => l.UserName != null).GroupBy(l => l.UserName!).ToDictionaryAsync(g => g.Key, g => g.Count()),
                DateStatistics = await query.GroupBy(l => l.CreateTime.Date.ToString("yyyy-MM-dd")).ToDictionaryAsync(g => g.Key, g => g.Count())
            };

            return ApiResult<AuditLogStatisticsDto>.Succeed(stats);
        }

        private IQueryable<AuditLog> ApplyFilters(IQueryable<AuditLog> query, AuditLogQueryDto parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query.Where(x => x.Description.Contains(parameters.SearchTerm) || (x.UserName != null && x.UserName.Contains(parameters.SearchTerm)));
            }
            if (!string.IsNullOrWhiteSpace(parameters.Action))
            {
                query = query.Where(x => x.Action == parameters.Action);
            }
            if (!string.IsNullOrWhiteSpace(parameters.Module))
            {
                query = query.Where(x => x.Module == parameters.Module);
            }
            if (parameters.UserId != null)
            {
                query = query.Where(x => x.UserId == parameters.UserId);
            }
            if (!string.IsNullOrWhiteSpace(parameters.Result))
            {
                query = query.Where(x => x.Result == parameters.Result);
            }
            if (parameters.DateFrom.HasValue)
            {
                query = query.Where(x => x.CreateTime >= parameters.DateFrom.Value);
            }
            if (parameters.DateTo.HasValue)
            {
                query = query.Where(x => x.CreateTime <= parameters.DateTo.Value);
            }
            if (!string.IsNullOrWhiteSpace(parameters.IpAddress))
            {
                query = query.Where(x => x.IpAddress == parameters.IpAddress);
            }
            if (!string.IsNullOrWhiteSpace(parameters.EntityType))
            {
                query = query.Where(x => x.EntityType == parameters.EntityType);
            }
            return query;
        }

        private Expression<Func<AuditLog, object>> GetSortExpression(string sortBy)
        {
            return sortBy.ToLowerInvariant() switch
            {
                "action" => x => x.Action,
                "module" => x => x.Module,
                "username" => x => x.UserName!,
                "duration" => x => x.Duration!,
                "statuscode" => x => x.ResponseStatusCode!,
                _ => x => x.CreateTime,
            };
        }


        private string GenerateCsvData(List<AuditLog> auditLogs)
        {
            var headers = new[] {
            "操作时间", "操作类型", "操作模块", "操作描述", "操作用户", "IP地址",
            "请求路径", "请求方法", "响应状态码", "操作结果", "错误信息", "操作耗时(ms)",
            "关联实体ID", "关联实体类型", "用户代理"
        };

            var csvRows = new List<string> { string.Join(",", headers.Select(h => $"\"{h}\"")) };

            foreach (var audit in auditLogs)
            {
                var row = new[]
                {
                $"\"{audit.CreateTime:yyyy-MM-dd HH:mm:ss}\"",
                $"\"{audit.Action}\"",
                $"\"{audit.Module}\"",
                $"\"{audit.Description}\"",
                $"\"{audit.UserName ?? "系统"}\"",
                $"\"{audit.IpAddress ?? ""}\"",
                $"\"{audit.RequestPath ?? ""}\"",
                $"\"{audit.RequestMethod ?? ""}\"",
                $"\"{audit.ResponseStatusCode?.ToString() ?? ""}\"",
                $"\"{audit.Result}\"",
                $"\"{audit.ErrorMessage ?? ""}\"",
                $"\"{audit.Duration?.ToString() ?? ""}\"",
                $"\"{audit.EntityId ?? ""}\"",
                $"\"{audit.EntityType ?? ""}\"",
                $"\"{audit.UserAgent ?? ""}\""
            };
                csvRows.Add(string.Join(",", row));
            }

            return string.Join("\n", csvRows);
        }
    }
}
