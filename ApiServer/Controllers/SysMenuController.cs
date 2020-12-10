using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using ApiServer.Model.Model.MsgModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysMenuController : ControllerBase
    {
        private readonly ISysMenuService _sysMenuService;

        public SysMenuController(ISysMenuService sysMenuService)
        {
            _sysMenuService = sysMenuService;
        }

        /// <summary>
        /// 菜单管理：查询
        /// </summary>
        /// <param name="menuNameLike"></param>
        /// <param name="menuStatus"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("tree")]
        public async Task<IActionResult> Tree([FromBody] string menuNameLike, bool menuStatus)
        {
            return Ok(await Task.FromResult(_sysMenuService.GetMenuTree(menuNameLike, menuStatus)));
        }

        /// <summary>
        /// 菜单管理：修改
        /// </summary>
        /// <param name="sys_Menu"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] Sys_Menu sys_Menu)
        {
            MsgModel msg = new MsgModel
            {
                message = "更新菜单项成功！"
            };
            _sysMenuService.UpdateMenu(sys_Menu);

            return Ok(await Task.FromResult(msg));

        }

        /// <summary>
        /// 菜单管理：新增
        /// </summary>
        /// <param name="sys_Menu"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Sys_Menu sys_Menu)
        {
            MsgModel msg = new MsgModel
            {
                message = "新增菜单项成功！"
            };
            _sysMenuService.AddMenu(sys_Menu);

            return Ok(await Task.FromResult(msg));

        }

        /// <summary>
        /// 菜单管理：新增
        /// </summary>
        /// <param name="sys_Menu"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] Sys_Menu sys_Menu)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除菜单项成功！"
            };
            _sysMenuService.DeleteMenu(sys_Menu);

            return Ok(await Task.FromResult(msg));

        }

        /// <summary>
        /// 角色管理:菜单树展示（勾选项、展开项）
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkedtree")]
        public async Task<IActionResult> CheckTree([FromBody] long roleId)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("tree", _sysMenuService.GetMenuTree("", default));
            dict.Add("expandedKeys", _sysMenuService.GetExpandedKeys());
            dict.Add("checkedKeys", _sysMenuService.GetCheckedKeys(roleId));

            return Ok(await Task.FromResult(dict));

        }

        /// <summary>
        /// 角色管理：保存菜单勾选结果
        /// </summary>
        /// <param name="roleCheckedIds"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("savekeys")]
        public async Task<IActionResult> SaveKeys([FromBody] RoleCheckedIds roleCheckedIds)
        {
            MsgModel msg = new MsgModel
            {
                message = "保存菜单权限成功！"
            };
            _sysMenuService.SaveCheckedKeys(roleCheckedIds.RoleId, roleCheckedIds.CheckedIds);

            return Ok(await Task.FromResult(msg));

        }

        /// <summary>
        /// 系统左侧菜单栏加载，根据登录用户名加载它可以访问的菜单项
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("tree/user")]
        public async Task<IActionResult> UserTree(string userName)
        {
            return Ok(await Task.FromResult(_sysMenuService.GetMenuTreeByUsername(userName))); ;

        }

        /// <summary>
        /// 菜单管理：更新菜单禁用状态
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("status/change")]
        public async Task<IActionResult> Update([FromBody] long menuId, bool status)
        {
            MsgModel msg = new MsgModel
            {
                message = "菜单禁用状态更新成功！"
            };
            _sysMenuService.UpdateStatus(menuId, status);

            return Ok(await Task.FromResult(msg));

        }
    }
}
