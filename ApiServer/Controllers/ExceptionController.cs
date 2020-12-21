using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ApiServer.Controllers
{
    /// <summary>
    /// 异常
    /// </summary>
    public class ExceptionController : BaseController
    {
        private readonly ILogger _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public ExceptionController(ILogger<ExceptionController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [Route("api/exception")]
        public IEnumerable<string> Get()
        {
            _logger.LogError("全局异常过滤测试");
            throw new System.Exception("自定义全局异常过滤抛出测试");

        }
    }
}
