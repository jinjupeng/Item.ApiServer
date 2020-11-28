using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreJWT.Controllers
{
    [ApiController]
    public class ExceptionController: ControllerBase
    {
        private readonly ILogger _logger;

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
