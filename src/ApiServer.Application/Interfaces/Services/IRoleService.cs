using ApiServer.Application.DTOs.Role;
using ApiServer.Shared.Common;

namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 角色服务接口
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// 创建角色
        /// </summary>
        Task<ApiResult<long>> CreateRoleAsync(CreateRoleDto dto);

        /// <summary>
        /// 更新角色
        /// </summary>
        Task<ApiResult> UpdateRoleAsync(long id, UpdateRoleDto dto);

        /// <summary>
        /// 删除角色
        /// </summary>
        Task<ApiResult> DeleteRoleAsync(long id);

        /// <summary>
        /// 获取角色详情
        /// </summary>
        Task<ApiResult<RoleDto>> GetRoleByIdAsync(long id);

        /// <summary>
        /// 分页查询角色
        /// </summary>
        Task<ApiResult<PagedResult<RoleDto>>> GetPagedRolesAsync(RoleQueryDto query);

        /// <summary>
        /// 获取所有角色
        /// </summary>
        Task<ApiResult<List<RoleDto>>> GetAllRolesAsync();

        /// <summary>
        /// 根据用户ID获取角色列表
        /// </summary>
        Task<ApiResult<List<RoleDto>>> GetRolesByUserIdAsync(long userId);

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        Task<ApiResult> AssignRolesToUserAsync(UserRoleAssignDto dto);

        /// <summary>
        /// 移除用户的角色
        /// </summary>
        Task<ApiResult> RemoveRolesFromUserAsync(long userId, List<long> roleIds);

        /// <summary>
        /// 检查角色名称是否存在
        /// </summary>
        Task<ApiResult<bool>> RoleNameExistsAsync(string roleName, long? excludeId = null);

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        Task<ApiResult> AssignPermissionsToRoleAsync(RolePermissionDto dto);

        /// <summary>
        /// 获取角色权限
        /// </summary>
        Task<ApiResult<RolePermissionDto>> GetRolePermissionsAsync(long roleId);

        /// <summary>
        /// 更新角色状态
        /// </summary>
        Task<ApiResult> UpdateRoleStatusAsync(long id, bool status);

        /// <summary>
        /// 获取用户角色选择列表（用于分配角色时显示）
        /// </summary>
        Task<ApiResult<List<RoleDto>>> GetRolesForUserAssignAsync(long userId);
    }
}
