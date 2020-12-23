using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiServer.Model.Model.ViewModel;
using Mapster;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class SysOrgController : BaseController
    {
        private readonly ISysOrgService _sysOrgService;
        private readonly ISysUserService _sysUserService;

        public SysOrgController(ISysOrgService sysOrgService, ISysUserService sysUserService)
        {
            _sysOrgService = sysOrgService;
            _sysUserService = sysUserService;
        }

        [HttpPost]
        [Route("tree")]
        public async Task<IActionResult> Tree([FromForm] Dictionary<string, string> pairs)
        {
            string userName = (string)pairs["username"];
            string orgNameLike = (string)pairs["orgNameLike"];
            bool? orgStatus = null;
            if (!string.IsNullOrWhiteSpace(pairs["orgStatus"]))
            {
                orgStatus = Convert.ToBoolean(pairs["orgStatus"]);
            }
            Sys_User sys_User = _sysUserService.GetUserByUserName(userName);
            return Ok(await Task.FromResult(_sysOrgService.GetOrgTreeById(sys_User.org_id, orgNameLike, orgStatus)));
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] Sys_Org sys_Org)
        {

            return Ok(await Task.FromResult(_sysOrgService.UpdateOrg(sys_Org)));

        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] SysOrg sysOrg)
        {
            TypeAdapterConfig<SysOrg, Sys_Org>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            var sys_Org = sysOrg.BuildAdapter().AdaptToType<Sys_Org>();
            return Ok(await Task.FromResult(_sysOrgService.AddOrg(sys_Org)));

        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] Sys_Org sys_Org)
        {
            return Ok(await Task.FromResult(_sysOrgService.DeleteOrg(sys_Org)));

        }

        [HttpPost]
        [Route("status/change")]
        public async Task<IActionResult> Update([FromForm] long orgId, bool status)
        {
            return Ok(await Task.FromResult(_sysOrgService.UpdateStatus(orgId, status)));

        }
    }
}
