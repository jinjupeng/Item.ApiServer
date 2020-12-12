using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysUserController : ControllerBase
    {
        private readonly ISysUserService _sysUserService;

        public SysUserController(ISysUserService sysUserService)
        {
            _sysUserService = sysUserService;
        }

        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> Info([FromQuery] string userName)
        {
            return Ok(await Task.FromResult(_sysUserService.GetUserByUserName(userName)));
        }

        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> Query([FromForm] long orgId, string userName, string phone, string email, bool enabled, DateTime createStartTime, DateTime createEndTime, int pageNum, int pageSize)
        {
            // TODO：
            return Ok(await Task.FromResult(1));
        }

        /// <summary>
        /// 用户管理：更新
        /// </summary>
        /// <param name="sys_User"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody]Sys_User sys_User)
        {
            MsgModel msg = new MsgModel
            {
                message = "更新用户成功！"
            };
            _sysUserService.UpdateUser(sys_User);
            return Ok(await Task.FromResult(msg));
        }
    }
}
