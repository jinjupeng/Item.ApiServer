using ApiServer.Shared.Common;
using System.Net;
using System.Text.Json;

namespace ApiServer.WebApi.Middlewares
{
    /// <summary>
    /// 全局异常处理中间件
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 检查响应是否已经开始或完成
            if (context.Response.HasStarted)
            {
                return;
            }

            try
            {
                context.Response.ContentType = "application/json";
                
                var response = new ApiResult();
                
                switch (exception)
                {
                    case ArgumentException:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response = ApiResult.Failed("请求参数错误");
                        break;
                    
                    case UnauthorizedAccessException:
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        response = ApiResult.Failed("未授权访问");
                        break;
                    
                    case KeyNotFoundException:
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        response = ApiResult.Failed("资源未找到");
                        break;
                    
                    case TimeoutException:
                        context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                        response = ApiResult.Failed("请求超时");
                        break;
                    
                    case ObjectDisposedException:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response = ApiResult.Failed("请求已被取消或连接已断开");
                        break;
                    
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response = ApiResult.Failed("服务器内部错误");
                        break;
                }

                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                await context.Response.WriteAsync(jsonResponse);
            }
            catch (ObjectDisposedException)
            {
                // 如果在写入响应时发生ObjectDisposedException，说明连接已断开，忽略此异常
                return;
            }
            catch (Exception)
            {
                // 如果在异常处理过程中再次发生异常，忽略以避免无限循环
                return;
            }
        }
    }
}