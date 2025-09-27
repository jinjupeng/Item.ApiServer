using System.Diagnostics;

namespace ApiServer.WebApi.Middlewares
{
    /// <summary>
    /// 请求响应日志中间件
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();
            
            // 记录请求信息
            var request = context.Request;
            _logger.LogInformation(
                "Request {RequestId} started: {Method} {Path} from {RemoteIpAddress}",
                requestId,
                request.Method,
                request.Path,
                context.Connection.RemoteIpAddress);

            // 如果是POST/PUT请求且内容类型为JSON，记录请求体
            if (ShouldLogRequestBody(request))
            {
                var requestBody = await ReadRequestBodyAsync(request);
                if (!string.IsNullOrEmpty(requestBody))
                {
                    _logger.LogInformation(
                        "Request {RequestId} body: {RequestBody}",
                        requestId,
                        requestBody);
                }
            }

            var originalBodyStream = context.Response.Body;
            
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                await _next(context);
                
                stopwatch.Stop();
                
                // 记录响应信息
                var response = context.Response;
                _logger.LogInformation(
                    "Request {RequestId} completed: {StatusCode} in {ElapsedMilliseconds}ms",
                    requestId,
                    response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                // 如果响应内容类型为JSON，记录响应体
                if (ShouldLogResponseBody(response))
                {
                    var responseBody = await ReadResponseBodyAsync(responseBodyStream);
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        _logger.LogInformation(
                            "Request {RequestId} response: {ResponseBody}",
                            requestId,
                            responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Request {RequestId} failed after {ElapsedMilliseconds}ms",
                    requestId,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalBodyStream);
            }
        }

        private static bool ShouldLogRequestBody(HttpRequest request)
        {
            return (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH") &&
                   request.ContentType?.Contains("application/json") == true &&
                   request.ContentLength > 0 && request.ContentLength < 10240; // 小于10KB
        }

        private static bool ShouldLogResponseBody(HttpResponse response)
        {
            return response.ContentType?.Contains("application/json") == true &&
                   response.ContentLength > 0 && response.ContentLength < 10240; // 小于10KB
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            
            request.Body.Position = 0;
            return body;
        }

        private static async Task<string> ReadResponseBodyAsync(Stream responseBodyStream)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            
            using var reader = new StreamReader(responseBodyStream, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}