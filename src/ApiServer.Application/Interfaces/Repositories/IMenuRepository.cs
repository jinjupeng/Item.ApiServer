using ApiServer.Domain.Entities;

namespace ApiServer.Application.Interfaces.Repositories
{
    /// <summary>
    /// 菜单仓储接口
    /// </summary>
    public interface IMenuRepository : IBaseRepository<Permission>
    {
        /// <summary>
        /// 根据菜单编码获取菜单
        /// </summary>
        Task<Permission?> GetByMenuCodeAsync(string menuCode);

        /// <summary>
        /// 根据用户ID获取菜单列表
        /// </summary>
        Task<IEnumerable<Permission>> GetMenusByUserIdAsync(long userId);

        /// <summary>
        /// 根据角色ID获取菜单列表
        /// </summary>
        Task<IEnumerable<Permission>> GetMenusByRoleIdAsync(long roleId);

        /// <summary>
        /// 检查菜单编码是否存在
        /// </summary>
        Task<bool> IsMenuCodeExistsAsync(string menuCode, long? excludeMenuId = null);

        /// <summary>
        /// 获取菜单树
        /// </summary>
        Task<IEnumerable<Permission>> GetMenuTreeAsync(string? menuName = null, bool? status = null);

        /// <summary>
        /// 根据父ID获取子菜单
        /// </summary>
        Task<IEnumerable<Permission>> GetChildMenusAsync(long parentId);

        /// <summary>
        /// 获取父菜单列表
        /// </summary>
        Task<IEnumerable<Permission>> GetParentMenusAsync();

        /// <summary>
        /// 更新菜单排序
        /// </summary>
        Task UpdateMenuSortAsync(long menuId, int sort);

        /// <summary>
        /// 获取最大排序值
        /// </summary>
        Task<int> GetMaxSortAsync(long? parentId = null);

        /// <summary>
        /// 检查是否有子菜单
        /// </summary>
        Task<bool> HasChildMenusAsync(long menuId);

        /// <summary>
        /// 获取所有父菜单（非叶子节点）
        /// </summary>
        Task<IEnumerable<Permission>> GetAllParentMenusAsync();

        /// <summary>
        /// 根据URL获取菜单
        /// </summary>
        Task<Permission?> GetByUrlAsync(string url);

        /// <summary>
        /// 批量更新菜单状态
        /// </summary>
        Task BatchUpdateStatusAsync(IEnumerable<long> menuIds, bool status);
    }
}
