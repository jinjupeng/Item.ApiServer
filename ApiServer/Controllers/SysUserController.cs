using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class SysUserController : BaseController
    {
        private readonly ISysUserService _sysUserService;

        public SysUserController(ISysUserService sysUserService)
        {
            _sysUserService = sysUserService;
        }

        /// <summary>
        /// 获取用户信息接口(个人中心)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> Info([FromQuery] string userName)
        {
            return Ok(await Task.FromResult(_sysUserService.GetUserByUserName(userName)));
        }

        /// <summary>
        /// 用户列表查询接口
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> Query([FromForm] Dictionary<string, string> pairs)
        {
            long? orgId = null;//long.TryParse(pairs["orgId"], out long tryOrgId) ? tryOrgId : default;
            string userName = pairs["username"];
            string phone = pairs["phone"];
            string email = pairs["email"];
            bool? enabled = null;//Convert.ToBoolean(pairs["enabled"]);
            DateTime? createStartTime = null; // Convert.ToDateTime(pairs["createStartTime"]);
            DateTime? createEndTime = null; // Convert.ToDateTime(pairs["createEndTime"]);
            int pageNum = Convert.ToInt32(pairs["pageNum"]);
            int pageSize = Convert.ToInt32(pairs["pageSize"]);
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
            return Ok(await Task.FromResult(_sysUserService.UpdateUser(sys_User)));
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
            return Ok(await Task.FromResult(_sysUserService.AddUser(sys_User)));
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
            return Ok(await Task.FromResult(_sysUserService.DeleteUser(userId)));
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
            return Ok(await Task.FromResult(_sysUserService.UpdateEnabled(userId, enabled)));
        }
    }
}
