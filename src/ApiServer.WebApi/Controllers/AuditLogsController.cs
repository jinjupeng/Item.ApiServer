using ApiServer.Application.Interfaces.Services;
using ApiServer.Application.DTOs.Audit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiServer.Shared.Common;
using ApiServer.Application.Interfaces;

namespace ApiServer.WebApi.Controllers
{
    /// <summary>
    /// 审计日志控制器
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class AuditLogsController : BaseController
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUser _currentUser;

        public AuditLogsController(IAuditLogService auditLogService, ICurrentUser currentUser)
        {
            _auditLogService = auditLogService;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 获取审计日志分页列表
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogQueryDto parameters)
        {
            var result = await _auditLogService.GetAuditLogsAsync(parameters);
            return HandleResult(result);
        }

        /// <summary>
        /// 根据ID获取审计日志详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditLogById(long id)
        {
            var result = await _auditLogService.GetAuditLogByIdAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取用户操作日志
        /// </summary>
        [HttpGet("my-logs")]
        public async Task<IActionResult> GetMyAuditLogs([FromQuery] AuditLogQueryDto parameters)
        {
            long? userId = _currentUser.UserId;
            if (userId == null)
            {
                return Fail("无法获取当前用户ID");
            }
            var result = await _auditLogService.GetUserAuditLogsAsync(userId.Value, parameters);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取指定用户的操作日志
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserAuditLogs(
            long userId, [FromQuery] AuditLogQueryDto parameters)
        {
            var result = await _auditLogService.GetUserAuditLogsAsync(userId, parameters);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        /// <summary>
        /// 获取模块操作日志
        /// </summary>
        [HttpGet("module/{module}")]
        public async Task<IActionResult> GetModuleAuditLogs(string module, [FromQuery] AuditLogQueryDto parameters)
        {
            var result = await _auditLogService.GetModuleAuditLogsAsync(module, parameters);
            return HandleResult(result);
        }


        /// <summary>
        /// 导出审计日志
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> ExportAuditLogs([FromQuery] AuditLogExportDto parameters)
        {
            try
            {
                var result = await _auditLogService.ExportAuditLogsAsync(parameters);
                if (!result.Success)
                {
                    return BadRequest(new ApiResult<string>
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                var fileName = $"system-audit-logs-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.csv";
                return File(System.Text.Encoding.UTF8.GetBytes(result.Data), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResult<string>
                {
                    Success = false,
                    Message = "导出审计日志失败" + ex.Message
                });
            }
        }


        /// <summary>
        /// 获取审计日志统计信息
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetAuditLogStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _auditLogService.GetAuditLogStatisticsAsync(startDate, endDate);
            return HandleResult(result);
        }

        /// <summary>
        /// 清理过期审计日志
        /// </summary>
        [HttpDelete("cleanup")]
        public async Task<IActionResult> CleanupExpiredLogs([FromQuery] int retentionDays = 90)
        {
            var result = await _auditLogService.CleanupExpiredLogsAsync(retentionDays);
            return HandleResult(result);
        }


        /// <summary>
        /// 手动记录审计日志
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> LogAudit([FromBody] AuditLogDto auditLog)
        {
            try
            {
                await _auditLogService.LogAsync(auditLog);
                return Ok(new ApiResult<bool>
                {
                    Success = true,
                    Message = "审计日志记录成功",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResult<bool>
                {
                    Success = false,
                    Message = "记录审计日志失败" + ex.Message
                });
            }
        }
    }
}
