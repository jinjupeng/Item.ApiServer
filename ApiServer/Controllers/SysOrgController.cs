using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Tree([FromForm] string userName, string orgNameLike, bool orgStatus)
        {
            Sys_User sys_User = _sysUserService.GetUserByUserName(userName);
            return Ok(await Task.FromResult(_sysOrgService.GetOrgTreeById(sys_User.org_id, orgNameLike, orgStatus)));
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] Sys_Org sys_Org)
        {
            MsgModel msg = new MsgModel
            {
                message = "更新组织机构成功！"
            };
            _sysOrgService.UpdateOrg(sys_Org);

            return Ok(await Task.FromResult(msg));

        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Sys_Org sys_Org)
        {
            MsgModel msg = new MsgModel
            {
                message = "新增组织机构成功！"
            };
            _sysOrgService.AddOrg(sys_Org);

            return Ok(await Task.FromResult(msg));

        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] Sys_Org sys_Org)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除组织机构成功！"
            };
            _sysOrgService.DeleteOrg(sys_Org);

            return Ok(await Task.FromResult(msg));

        }

        [HttpPost]
        [Route("status/change")]
        public async Task<IActionResult> Update([FromForm] long orgId, bool status)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除组织机构成功！"
            };
            _sysOrgService.UpdateStatus(orgId, status);

            return Ok(await Task.FromResult(msg));

        }
    }
}
