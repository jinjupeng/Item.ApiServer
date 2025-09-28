using ApiServer.Domain.Entities;

namespace ApiServer.Application.Interfaces.Repositories
{
    /// <summary>
    /// 角色仓储接口
    /// </summary>
    public interface IRoleRepository : IBaseRepository<Role>
    {
        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        Task<Role?> GetByRoleNameAsync(string roleName);

        /// <summary>
        /// 根据用户ID获取角色列表
        /// </summary>
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(long userId);

        /// <summary>
        /// 检查角色名称是否存在
        /// </summary>
        Task<bool> IsRoleNameExistsAsync(string roleName, long? excludeRoleId = null);

        /// <summary>
        /// 分页查询角色
        /// </summary>
        Task<(IEnumerable<Role> roles, int total)> GetPagedRolesAsync(
            int page, 
            int pageSize, 
            string? roleName = null, 
            bool? status = null);

        /// <summary>
        /// 获取角色的菜单权限
        /// </summary>
        Task<IEnumerable<Permission>> GetRoleMenusAsync(long roleId);

        /// <summary>
        /// 获取角色的API权限
        /// </summary>
        Task<IEnumerable<Permission>> GetRoleApisAsync(long roleId);

        /// <summary>
        /// 为角色分配菜单权限
        /// </summary>
        Task AssignMenusToRoleAsync(long roleId, IEnumerable<long> menuIds);

        /// <summary>
        /// 为角色分配API权限
        /// </summary>
        Task AssignApisToRoleAsync(long roleId, IEnumerable<long> apiIds);

        /// <summary>
        /// 移除角色的菜单权限
        /// </summary>
        Task RemoveRoleMenusAsync(long roleId);

        /// <summary>
        /// 移除角色的API权限
        /// </summary>
        Task RemoveRoleApisAsync(long roleId);

        /// <summary>
        /// 获取角色下的用户数量
        /// </summary>
        Task<int> GetUserCountByRoleIdAsync(long roleId);

        /// <summary>
        /// 批量更新角色状态
        /// </summary>
        Task BatchUpdateStatusAsync(IEnumerable<long> roleIds, bool status);
    }
}
