using ApiServer.Domain.Entities;

namespace ApiServer.Application.Interfaces.Repositories
{
    /// <summary>
    /// 菜单仓储接口
    /// </summary>
    public interface IMenuRepository : IBaseRepository<Menu>
    {
        /// <summary>
        /// 根据菜单编码获取菜单
        /// </summary>
        Task<Menu?> GetByMenuCodeAsync(string menuCode);

        /// <summary>
        /// 根据用户ID获取菜单列表
        /// </summary>
        Task<IEnumerable<Menu>> GetMenusByUserIdAsync(long userId);

        /// <summary>
        /// 根据角色ID获取菜单列表
        /// </summary>
        Task<IEnumerable<Menu>> GetMenusByRoleIdAsync(long roleId);

        /// <summary>
        /// 检查菜单编码是否存在
        /// </summary>
        Task<bool> IsMenuCodeExistsAsync(string menuCode, long? excludeMenuId = null);

        /// <summary>
        /// 获取菜单树
        /// </summary>
        Task<IEnumerable<Menu>> GetMenuTreeAsync(string? menuName = null, bool? status = null);

        /// <summary>
        /// 根据父ID获取子菜单
        /// </summary>
        Task<IEnumerable<Menu>> GetChildMenusAsync(long parentId);

        /// <summary>
        /// 获取父菜单列表
        /// </summary>
        Task<IEnumerable<Menu>> GetParentMenusAsync();

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
        Task<IEnumerable<Menu>> GetAllParentMenusAsync();

        /// <summary>
        /// 根据URL获取菜单
        /// </summary>
        Task<Menu?> GetByUrlAsync(string url);

        /// <summary>
        /// 批量更新菜单状态
        /// </summary>
        Task BatchUpdateStatusAsync(IEnumerable<long> menuIds, bool status);
    }
}
