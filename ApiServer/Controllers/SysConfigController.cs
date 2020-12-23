using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class SysConfigController : BaseController
    {
        private readonly ISysConfigService _sysConfigService;
        public SysConfigController(ISysConfigService sysConfigService)
        {
            _sysConfigService = sysConfigService;
        }

        [HttpPost]
        [Route("all")]
        public async Task<IActionResult> All()
        {
            return Ok(await Task.FromResult(_sysConfigService.GetSysConfigList()));
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh()
        {
            return Ok(await Task.FromResult(_sysConfigService.GetSysConfigList()));
        }

        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> Query([FromForm] string configLike)
        {
            return Ok(await Task.FromResult(_sysConfigService.QueryConfigs(configLike)));
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] SysConfig sysConfig)
        {
            TypeAdapterConfig<SysConfig, Sys_Config>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            var sys_Config = sysConfig.BuildAdapter().AdaptToType<Sys_Config>();
            return Ok(await Task.FromResult(_sysConfigService.UpdateConfig(sys_Config)));

        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] SysConfig sysConfig)
        {
            TypeAdapterConfig<SysConfig, Sys_Config>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            var sys_Config = sysConfig.BuildAdapter().AdaptToType<Sys_Config>();
            return Ok(await Task.FromResult(_sysConfigService.AddConfig(sys_Config)));

        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromForm] long configId)
        {
            return Ok(await Task.FromResult(_sysConfigService.DeleteConfig(configId)));

        }
    }
}
