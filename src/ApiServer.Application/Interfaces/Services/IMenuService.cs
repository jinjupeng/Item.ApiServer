using ApiServer.Application.DTOs.Menu;
using ApiServer.Shared.Common;

namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 菜单服务接口
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// 创建菜单
        /// </summary>
        Task<ApiResult<long>> CreateMenuAsync(CreateMenuDto dto);

        /// <summary>
        /// 更新菜单
        /// </summary>
        Task<ApiResult> UpdateMenuAsync(long id, UpdateMenuDto dto);

        /// <summary>
        /// 删除菜单
        /// </summary>
        Task<ApiResult> DeleteMenuAsync(long id);

        /// <summary>
        /// 获取菜单详情
        /// </summary>
        Task<ApiResult<MenuDto>> GetMenuByIdAsync(long id);

        /// <summary>
        /// 获取菜单树
        /// </summary>
        Task<ApiResult<List<MenuTreeDto>>> GetMenuTreeAsync(MenuQueryDto? query = null);

        /// <summary>
        /// 根据用户ID获取用户菜单树
        /// </summary>
        Task<ApiResult<List<MenuTreeDto>>> GetUserMenuTreeAsync(long userId);

        /// <summary>
        /// 根据用户名获取用户菜单树
        /// </summary>
        Task<ApiResult<List<MenuTreeDto>>> GetUserMenuTreeByUsernameAsync(string username);

        /// <summary>
        /// 根据角色ID获取角色菜单
        /// </summary>
        Task<ApiResult<List<MenuDto>>> GetMenusByRoleIdAsync(long roleId);

        /// <summary>
        /// 检查菜单编码是否存在
        /// </summary>
        Task<ApiResult<bool>> MenuCodeExistsAsync(string menuCode, long? excludeId = null);

        /// <summary>
        /// 获取父菜单列表
        /// </summary>
        Task<ApiResult<List<MenuDto>>> GetParentMenusAsync();

        /// <summary>
        /// 根据父ID获取子菜单
        /// </summary>
        Task<ApiResult<List<MenuDto>>> GetChildMenusAsync(long parentId);

        /// <summary>
        /// 更新菜单排序
        /// </summary>
        Task<ApiResult> UpdateMenuSortAsync(long menuId, int sort);

        /// <summary>
        /// 更新菜单状态
        /// </summary>
        Task<ApiResult> UpdateMenuStatusAsync(long id, bool status);

        /// <summary>
        /// 获取菜单选择树（用于角色分配菜单权限）
        /// </summary>
        Task<ApiResult<List<MenuTreeDto>>> GetMenuTreeForRoleAssignAsync(long? roleId = null);

        /// <summary>
        /// 保存角色菜单权限
        /// </summary>
        Task<ApiResult> SaveRoleMenuPermissionsAsync(long roleId, List<long> menuIds);

        /// <summary>
        /// 获取展开的菜单Keys
        /// </summary>
        Task<ApiResult<List<string>>> GetExpandedKeysAsync();

        /// <summary>
        /// 获取角色已选中的菜单Keys
        /// </summary>
        Task<ApiResult<List<string>>> GetCheckedKeysByRoleIdAsync(long roleId);

        /// <summary>
        /// 获取当前用户菜单树
        /// </summary>
        Task<ApiResult<List<MenuTreeDto>>> GetCurrentUserMenuTreeAsync();
    }
}
