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
        public async Task<IActionResult> Query([FromForm] long? orgId, string userName, string phone, string email, bool? enabled, DateTime? createStartTime, DateTime? createEndTime, int pageNum, int pageSize)
        {
            var result = _sysUserService.QueryUser(orgId, userName, phone, email, enabled, createStartTime, createEndTime, pageNum, pageSize);
            return Ok(await Task.FromResult(result));
        }

        /// <summary>
        /// 用户管理：更新
        /// </summary>
        /// <param name="sys_User"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] Sys_User sys_User)
        {
            MsgModel msg = new MsgModel
            {
                message = "更新用户成功！"
            };
            _sysUserService.UpdateUser(sys_User);
            return Ok(await Task.FromResult(msg));
        }

        /// <summary>
        /// 用户管理：新增
        /// </summary>
        /// <param name="sys_User"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Sys_User sys_User)
        {
            MsgModel msg = new MsgModel
            {
                message = "新增用户成功！"
            };
            _sysUserService.AddUser(sys_User);
            return Ok(await Task.FromResult(msg));
        }

        /// <summary>
        /// 用户管理：删除
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromForm] long userId)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除用户成功！"
            };
            _sysUserService.DeleteUser(userId);
            return Ok(await Task.FromResult(msg));
        }

        /// <summary>
        /// 用户管理：重置密码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("pwd/reset")]
        public async Task<IActionResult> PwdReset([FromForm] long userId)
        {
            return Ok(await Task.FromResult(_sysUserService.PwdReset(userId)));
        }

        /// <summary>
        /// 判断登录用户密码是否是默认密码
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("pwd/isdefault")]
        public async Task<IActionResult> Isdefault([FromForm] string userName)
        {

            return Ok(await Task.FromResult(_sysUserService.IsDefault(userName)));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="oldPass"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("pwd/change")]
        public async Task<IActionResult> PwdChange([FromForm] string userName, string oldPass, string newPass)
        {
            return Ok(await Task.FromResult(_sysUserService.ChangePwd(userName, oldPass, newPass)));
        }

        /// <summary>
        /// 用户管理：更新用户激活状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("enabled/change")]
        public async Task<IActionResult> Update([FromForm] long userId, bool enabled)
        {
            MsgModel msg = new MsgModel
            {
                message = "用户状态更新成功！"
            };
            _sysUserService.UpdateEnabled(userId, enabled);
            return Ok(await Task.FromResult(msg));
        }
    }
}
