using ApiServer.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    /// <summary>
    /// 基础控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// 返回成功结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>API结果</returns>
        protected IActionResult Success(string message = "操作成功")
        {
            return Ok(ApiResult.SuccessResult(message));
        }

        /// <summary>
        /// 返回成功结果（带数据）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns>API结果</returns>
        protected IActionResult Success<T>(T data, string message = "操作成功")
        {
            return Ok(ApiResult<T>.SuccessResult(data, message));
        }

        /// <summary>
        /// 返回失败结果
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <returns>API结果</returns>
        protected IActionResult Fail(string message)
        {
            return BadRequest(ApiResult.FailResult(message));
        }

        /// <summary>
        /// 返回失败结果（带数据）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="message">错误消息</param>
        /// <returns>API结果</returns>
        protected IActionResult Fail<T>(string message)
        {
            return BadRequest(ApiResult<T>.FailResult(message));
        }

        /// <summary>
        /// 处理业务结果
        /// </summary>
        /// <param name="result">业务结果</param>
        /// <returns>API结果</returns>
        protected IActionResult HandleResult(ApiResult result)
        {
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// 处理业务结果（带数据）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="result">业务结果</param>
        /// <returns>API结果</returns>
        protected IActionResult HandleResult<T>(ApiResult<T> result)
        {
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        /// <returns>用户ID</returns>
        protected long? GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User?.FindFirst("UserId")?.Value;
            return long.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// 获取当前用户名
        /// </summary>
        /// <returns>用户名</returns>
        protected string? GetCurrentUsername()
        {
            return HttpContext.User?.FindFirst("Username")?.Value;
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        protected string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            // 检查是否通过代理
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                ipAddress = forwardedFor.Split(',').FirstOrDefault()?.Trim();
            }
            
            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                ipAddress = realIp;
            }

            return ipAddress ?? "Unknown";
        }
    }
}