using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysConfigController : ControllerBase
    {
        private readonly ISysConfigService _sysConfigService;
        public SysConfigController(ISysConfigService sysConfigService)
        {
            _sysConfigService = sysConfigService;
        }

        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> Query([FromForm] string configLike)
        {
            return Ok(await Task.FromResult(_sysConfigService.QueryConfigs(configLike)));
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] Sys_Config sys_Config)
        {
            MsgModel msg = new MsgModel
            {
                message = "更新配置成功！"
            };
            _sysConfigService.UpdateConfig(sys_Config);

            return Ok(await Task.FromResult(msg));

        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Sys_Config sys_Config)
        {
            MsgModel msg = new MsgModel
            {
                message = "新增配置成功！"
            };
            _sysConfigService.AddConfig(sys_Config);

            return Ok(await Task.FromResult(msg));

        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromForm] long configId)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除配置成功！"
            };
            _sysConfigService.DeleteConfig(configId);

            return Ok(await Task.FromResult(msg));

        }
    }
}
