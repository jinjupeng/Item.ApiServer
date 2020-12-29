using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ApiServer.Middleware
{
    /// <summary>
    /// 统一异常处理
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        /// <param name="environment"></param>
        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            this._next = next;
            this._environment = environment;
        }

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                // var features = context.Features;
            }
            catch (System.Exception e)
            {
                await HandleException(context, e);
            }
        }

        private async Task HandleException(HttpContext context, System.Exception e)
        {

            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/json;charset=utf-8;";
            string error;

            if (_environment.IsDevelopment())
            {
                var json = new { message = e.Message };
                error = JsonConvert.SerializeObject(json);
            }
            else
                error = "抱歉，出错了";
            // TODO：将异常信息写入日志文件中

            await context.Response.WriteAsync(error);
        }
    }
}