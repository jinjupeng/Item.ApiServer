using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Middlewares
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        private SortedDictionary<string, object> _data;
        private Stopwatch _stopwatch;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            _stopwatch.Restart();
            _data = new SortedDictionary<string, object>();

            HttpRequest request = context.Request;
            _data.Add("Request.Url", request.Path.ToString());
            _data.Add("Request.Headers", request.Headers.ToDictionary(x => x.Key, v => string.Join(";", v.Value.ToList())));
            _data.Add("Request.Method", request.Method);
            _data.Add("Request.ExecuteStartTime", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            // 获取请求body内容
            if (request.Method.ToLower().Equals("post"))
            {
                // 启用倒带功能，就可以让 Request.Body 可以再次读取
                request.EnableBuffering();

                Stream stream = request.Body;
                byte[] buffer = new byte[request.ContentLength.Value];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                _data.Add("Request.Body", Encoding.UTF8.GetString(buffer));

                request.Body.Position = 0;
            }
            else if (request.Method.ToLower().Equals("get"))
            {
                _data.Add("Request.Body", request.QueryString.Value);
            }

            // 获取Response.Body内容
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                _data.Add("Response.Body", await GetResponse(context.Response));
                _data.Add("Response.ExecuteEndTime", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }

            // 响应完成记录时间和存入日志
            context.Response.OnCompleted(() =>
            {
                _stopwatch.Stop();
                _data.Add("ElaspedTime", _stopwatch.ElapsedMilliseconds + "ms");
                var json = JsonConvert.SerializeObject(_data);
                _logger.LogInformation(json, "api", request.Method.ToUpper());
                return Task.CompletedTask;
            });

        }

        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<string> GetResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }
    /// <summary>
    /// 扩展中间件
    /// </summary>
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
