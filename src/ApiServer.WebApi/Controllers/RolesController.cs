using ApiServer.Application.DTOs.Role;
using ApiServer.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    /// <summary>
    /// 角色管理控制器
    /// </summary>
    [Authorize]
    public class RolesController : BaseController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>角色列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetRoles([FromQuery] RoleQueryDto query)
        {
            var result = await _roleService.GetPagedRolesAsync(query);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns>所有角色列表</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();
            return HandleResult(result);
        }

        /// <summary>
        /// 根据ID获取角色详情
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>角色详情</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(long id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="dto">创建角色DTO</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            var result = await _roleService.CreateRoleAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="dto">更新角色DTO</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleDto dto)
        {
            var result = await _roleService.UpdateRoleAsync(id, dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(long id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// 更新角色状态
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <param name="status">状态</param>
        /// <returns>更新结果</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateRoleStatus(long id, [FromBody] bool status)
        {
            var result = await _roleService.UpdateRoleStatusAsync(id, status);
            return HandleResult(result);
        }

        /// <summary>
        /// 检查角色名称是否存在
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="excludeId">排除的角色ID</param>
        /// <returns>检查结果</returns>
        [HttpGet("check-name")]
        public async Task<IActionResult> CheckRoleName([FromQuery] string roleName, [FromQuery] long? excludeId = null)
        {
            var result = await _roleService.RoleNameExistsAsync(roleName, excludeId);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户角色列表</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(long userId)
        {
            var result = await _roleService.GetRolesByUserIdAsync(userId);
            return HandleResult(result);
        }

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="dto">用户角色分配DTO</param>
        /// <returns>分配结果</returns>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRolesToUser([FromBody] UserRoleAssignDto dto)
        {
            var result = await _roleService.AssignRolesToUserAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 移除用户的角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleIds">角色ID列表</param>
        /// <returns>移除结果</returns>
        [HttpDelete("user/{userId}/roles")]
        public async Task<IActionResult> RemoveRolesFromUser(long userId, [FromBody] List<long> roleIds)
        {
            var result = await _roleService.RemoveRolesFromUserAsync(userId, roleIds);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>角色权限</returns>
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetRolePermissions(long id)
        {
            var result = await _roleService.GetRolePermissionsAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="dto">角色权限DTO</param>
        /// <returns>分配结果</returns>
        [HttpPost("permissions")]
        public async Task<IActionResult> AssignPermissionsToRole([FromBody] RolePermissionDto dto)
        {
            var result = await _roleService.AssignPermissionsToRoleAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 获取用户角色选择列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色选择列表</returns>
        [HttpGet("user/{userId}/assign-list")]
        public async Task<IActionResult> GetRolesForUserAssign(long userId)
        {
            var result = await _roleService.GetRolesForUserAssignAsync(userId);
            return HandleResult(result);
        }
    }
}
