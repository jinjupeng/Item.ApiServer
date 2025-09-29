using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Services;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace ApiServer.WebApi.Middlewares
{
    /// <summary>
    /// 审计中间件
    /// </summary>
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLogMiddleware> _logger;
        private readonly ICurrentUser _currentUser;

        public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger, ICurrentUser currentUser)
        {
            _next = next;
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
        {
            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            // 记录请求信息
            var requestInfo = await CaptureRequestInfoAsync(context);

            // 创建响应流来捕获响应
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            Exception? exception = null;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();

                // 记录响应信息
                var responseInfo = await CaptureResponseInfoAsync(context, responseBodyStream);

                // 记录审计日志
                await LogAuditAsync(auditLogService, context, requestInfo, responseInfo,
                    stopwatch.ElapsedMilliseconds, exception);

                // 恢复原始响应流
                await responseBodyStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task<RequestInfo> CaptureRequestInfoAsync(HttpContext context)
        {
            var request = context.Request;

            // 读取请求体
            string? requestBody = null;
            if (request.ContentLength > 0 &&
                (request.ContentType?.Contains("application/json") == true ||
                 request.ContentType?.Contains("application/x-www-form-urlencoded") == true))
            {
                request.EnableBuffering();
                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            return new RequestInfo
            {
                Method = request.Method,
                Path = request.Path.Value ?? "",
                QueryString = request.QueryString.Value ?? "",
                ContentType = request.ContentType ?? "",
                ContentLength = request.ContentLength ?? 0,
                Body = requestBody,
                IpAddress = GetClientIpAddress(context),
                UserAgent = request.Headers.UserAgent.ToString()
            };
        }

        private async Task<ResponseInfo> CaptureResponseInfoAsync(HttpContext context, MemoryStream responseBodyStream)
        {
            responseBodyStream.Position = 0;
            using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true);
            var responseBody = await reader.ReadToEndAsync();
            responseBodyStream.Position = 0;

            return new ResponseInfo
            {
                StatusCode = context.Response.StatusCode,
                ContentType = context.Response.ContentType ?? "",
                ContentLength = responseBody.Length,
                Body = responseBody
            };
        }

        private async Task LogAuditAsync(IAuditLogService auditLogService, HttpContext context,
            RequestInfo requestInfo, ResponseInfo responseInfo, long duration, Exception? exception)
        {
            try
            {
                // 跳过静态资源和健康检查
                if (ShouldSkipAudit(context.Request.Path))
                    return;

                // 确定操作类型和模块
                var (action, module, description) = DetermineActionAndModule(context.Request, responseInfo.StatusCode);

                // 记录审计日志
                await auditLogService.LogActionAsync(
                    action: action,
                    module: module,
                    description: description,
                    userId: _currentUser.UserId,
                    userName: _currentUser.UserName,
                    ipAddress: requestInfo.IpAddress,
                    userAgent: requestInfo.UserAgent,
                    requestPath: requestInfo.Path,
                    requestMethod: requestInfo.Method,
                    requestData: requestInfo.Body,
                    responseStatusCode: responseInfo.StatusCode,
                    duration: duration,
                    errorMessage: exception?.Message
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "记录审计日志失败");
            }
        }

        private bool ShouldSkipAudit(PathString path)
        {
            var skipPaths = new[]
            {
                "/health",
                "/swagger",
                "/favicon.ico",
                "/_framework",
                "/css",
                "/js",
                "/images",
                "/fonts"
            };

            return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath));
        }

        private (string action, string module, string description) DetermineActionAndModule(
            HttpRequest request, int statusCode)
        {
            var method = request.Method.ToUpper();
            var path = request.Path.Value?.ToLower() ?? "";

            // 确定操作类型
            var action = method switch
            {
                "GET" => "Read",
                "POST" => "Create",
                "PUT" => "Update",
                "PATCH" => "Update",
                "DELETE" => "Delete",
                _ => "Unknown"
            };

            // 确定模块
            var module = path switch
            {
                var p when p.Contains("/api/menus") => "Menu",
                var p when p.Contains("/api/roles") => "Role",
                var p when p.Contains("/api/auth") => "Auth",
                var p when p.Contains("/api/users") => "User",
                _ => "System"
            };

            // 生成描述
            var description = $"{action} {module} - {request.Path}";
            if (statusCode >= 400)
            {
                description += $" (Error: {statusCode})";
            }

            return (action, module, description);
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // 尝试从X-Forwarded-For头获取真实IP
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            // 尝试从X-Real-IP头获取
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // 使用连接远程IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private class RequestInfo
        {
            public string Method { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string QueryString { get; set; } = string.Empty;
            public string ContentType { get; set; } = string.Empty;
            public long ContentLength { get; set; }
            public string? Body { get; set; }
            public string IpAddress { get; set; } = string.Empty;
            public string UserAgent { get; set; } = string.Empty;
        }

        private class ResponseInfo
        {
            public int StatusCode { get; set; }
            public string ContentType { get; set; } = string.Empty;
            public int ContentLength { get; set; }
            public string Body { get; set; } = string.Empty;
        }
    }
}
