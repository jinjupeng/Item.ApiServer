using ApiServer.Application.DTOs.Menu;
using ApiServer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    /// <summary>
    /// 菜单管理控制器
    /// </summary>
    [Authorize]
    public class MenusController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>菜单树</returns>
        [HttpGet("tree")]
        public async Task<IActionResult> GetMenuTree([FromQuery] MenuQueryDto? query = null)
        {
            var result = await _menuService.GetMenuTreeAsync(query);
            return HandleResult(result);
        }

        /// <summary>
        /// 根据ID获取菜单详情
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>菜单详情</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenu(long id)
        {
            var result = await _menuService.GetMenuByIdAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="dto">创建菜单DTO</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto dto)
        {
            var result = await _menuService.CreateMenuAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="dto">更新菜单DTO</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(long id, [FromBody] UpdateMenuDto dto)
        {
            var result = await _menuService.UpdateMenuAsync(id, dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(long id)
        {
            var result = await _menuService.DeleteMenuAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// 更新菜单状态
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="status">状态</param>
        /// <returns>更新结果</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateMenuStatus(long id, [FromBody] bool status)
        {
            var result = await _menuService.UpdateMenuStatusAsync(id, status);
            return HandleResult(result);
        }

        /// <summary>
        /// 更新菜单排序
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="sort">排序值</param>
        /// <returns>更新结果</returns>
        [HttpPatch("{id}/sort")]
        public async Task<IActionResult> UpdateMenuSort(long id, [FromBody] int sort)
        {
            var result = await _menuService.UpdateMenuSortAsync(id, sort);
            return HandleResult(result);
        }

        /// <summary>
        /// 检查菜单编码是否存在
        /// </summary>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="excludeId">排除的菜单ID</param>
        /// <returns>检查结果</returns>
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckMenuCode([FromQuery] string menuCode, [FromQuery] long? excludeId = null)
        {
            var result = await _menuService.MenuCodeExistsAsync(menuCode, excludeId);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取父菜单列表
        /// </summary>
        /// <returns>父菜单列表</returns>
        [HttpGet("parents")]
        public async Task<IActionResult> GetParentMenus()
        {
            var result = await _menuService.GetParentMenusAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// 根据父ID获取子菜单
        /// </summary>
        /// <param name="parentId">父菜单ID</param>
        /// <returns>子菜单列表</returns>
        [HttpGet("parent/{parentId}/children")]
        public async Task<IActionResult> GetChildMenus(long parentId)
        {
            var result = await _menuService.GetChildMenusAsync(parentId);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取用户菜单树
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户菜单树</returns>
        [HttpGet("user/{userId}/tree")]
        public async Task<IActionResult> GetUserMenuTree(long userId)
        {
            var result = await _menuService.GetUserMenuTreeAsync(userId);
            return HandleResult(result);
        }

        /// <summary>
        /// 根据用户名获取用户菜单树
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户菜单树</returns>
        [HttpGet("user/{username}/tree-by-name")]
        public async Task<IActionResult> GetUserMenuTreeByUsername(string username)
        {
            var result = await _menuService.GetUserMenuTreeByUsernameAsync(username);
            return HandleResult(result);
        }

        /// <summary>
        /// 根据角色ID获取角色菜单
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>角色菜单列表</returns>
        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetMenusByRoleId(long roleId)
        {
            var result = await _menuService.GetMenusByRoleIdAsync(roleId);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取菜单选择树（用于角色分配菜单权限）
        /// </summary>
        /// <param name="roleId">角色ID（可选）</param>
        /// <returns>菜单选择树</returns>
        [HttpGet("assign-tree")]
        public async Task<IActionResult> GetMenuTreeForRoleAssign([FromQuery] long? roleId = null)
        {
            var result = await _menuService.GetMenuTreeForRoleAssignAsync(roleId);
            return HandleResult(result);
        }

        /// <summary>
        /// 保存角色菜单权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="menuIds">菜单ID列表</param>
        /// <returns>保存结果</returns>
        [HttpPost("role/{roleId}/permissions")]
        public async Task<IActionResult> SaveRoleMenuPermissions(long roleId, [FromBody] List<long> menuIds)
        {
            var result = await _menuService.SaveRoleMenuPermissionsAsync(roleId, menuIds);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取展开的菜单Keys
        /// </summary>
        /// <returns>展开的菜单Keys</returns>
        [HttpGet("expanded-keys")]
        public async Task<IActionResult> GetExpandedKeys()
        {
            var result = await _menuService.GetExpandedKeysAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// 获取角色已选中的菜单Keys
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>已选中的菜单Keys</returns>
        [HttpGet("role/{roleId}/checked-keys")]
        public async Task<IActionResult> GetCheckedKeysByRoleId(long roleId)
        {
            var result = await _menuService.GetCheckedKeysByRoleIdAsync(roleId);
            return HandleResult(result);
        }
    }
}
