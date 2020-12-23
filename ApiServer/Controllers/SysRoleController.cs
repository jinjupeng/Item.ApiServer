using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using ApiServer.Model.Model.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class SysRoleController : BaseController
    {
        private readonly ISysRoleService _sysRoleService;

        public SysRoleController(ISysRoleService sysRoleService)
        {
            _sysRoleService = sysRoleService;
        }

        /// <summary>
        /// 角色管理:查询
        /// </summary>
        /// <param name="roleLike"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> Query([FromForm] string roleLike)
        {
            return Ok(await Task.FromResult(_sysRoleService.QueryRoles(roleLike)));
        }

        /// <summary>
        /// 角色管理：修改
        /// </summary>
        /// <param name="sys_Role"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] SysRole sysRole)
        {
            TypeAdapterConfig<SysRole, Sys_Role>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            var sys_Role = sysRole.BuildAdapter().AdaptToType<Sys_Role>();
            return Ok(await Task.FromResult(_sysRoleService.UpdateRole(sys_Role)));

        }

        /// <summary>
        /// 角色管理：新增
        /// </summary>
        /// <param name="sys_Role"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] SysRole sysRole)
        {
            TypeAdapterConfig<SysRole, Sys_Role>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            var sys_Role = sysRole.BuildAdapter().AdaptToType<Sys_Role>();
            return Ok(await Task.FromResult(_sysRoleService.AddRole(sys_Role)));

        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromForm] long roleId)
        {
            return Ok(await Task.FromResult(_sysRoleService.DeleteRole(roleId)));

        }

        /// <summary>
        /// 用户管理：为用户分配角色，展示角色列表及勾选角色列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkedroles")]
        public async Task<IActionResult> CheckedRoles([FromForm] long userId)
        {
            return Ok(await Task.FromResult(_sysRoleService.GetRolesAndChecked(userId)));
        }

        /// <summary>
        /// 用户管理：保存用户角色
        /// </summary>
        /// <param name="userRoleCheckedIds"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("savekeys")]
        public async Task<IActionResult> Savekeys([FromBody] UserRoleCheckedIds userRoleCheckedIds)
        {
            return Ok(await Task.FromResult(_sysRoleService.SaveCheckedKeys(userRoleCheckedIds.UserId, userRoleCheckedIds.CheckedIds)));
        }

        /// <summary>
        /// 角色管理：更新角色禁用状态
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("status/change")]
        public async Task<IActionResult> Update([FromForm] long roleId, bool status)
        {
            return Ok(await Task.FromResult(_sysRoleService.UpdateStatus(roleId, status)));
        }
    }
}
