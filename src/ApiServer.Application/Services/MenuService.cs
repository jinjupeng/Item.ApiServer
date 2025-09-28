using ApiServer.Application.DTOs.Menu;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Domain.Entities;
using ApiServer.Shared.Common;
using Mapster;

namespace ApiServer.Application.Services
{
    /// <summary>
    /// 菜单服务实现
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MenuService(
            IMenuRepository menuRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        public async Task<ApiResult<long>> CreateMenuAsync(CreateMenuDto dto)
        {
            try
            {
                // 检查菜单编码是否已存在
                if (!string.IsNullOrEmpty(dto.MenuCode) && await _menuRepository.IsMenuCodeExistsAsync(dto.MenuCode))
                {
                    return ApiResult<long>.Failed("菜单编码已存在");
                }

                var menu = dto.Adapt<Permission>();
                
                // 设置菜单层级和路径信息
                await SetMenuHierarchyInfo(menu);

                await _menuRepository.AddAsync(menu);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult<long>.Succeed(menu.Id, "菜单创建成功");
            }
            catch (Exception ex)
            {
                return ApiResult<long>.Failed($"创建菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        public async Task<ApiResult> UpdateMenuAsync(long id, UpdateMenuDto dto)
        {
            try
            {
                var menu = await _menuRepository.GetByIdAsync(id);
                if (menu == null)
                {
                    return ApiResult.Failed("菜单不存在");
                }

                // 检查菜单编码是否已存在（排除当前菜单）
                if (!string.IsNullOrEmpty(dto.MenuCode) && await _menuRepository.IsMenuCodeExistsAsync(dto.MenuCode, id))
                {
                    return ApiResult.Failed("菜单编码已存在");
                }

                // 更新菜单信息
                dto.Adapt(menu);
                
                // 重新设置菜单层级和路径信息
                await SetMenuHierarchyInfo(menu);

                await _menuRepository.UpdateAsync(menu);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("菜单更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"更新菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        public async Task<ApiResult> DeleteMenuAsync(long id)
        {
            try
            {
                var menu = await _menuRepository.GetByIdAsync(id);
                if (menu == null)
                {
                    return ApiResult.Failed("菜单不存在");
                }

                // 检查是否有子菜单
                if (await _menuRepository.HasChildMenusAsync(id))
                {
                    return ApiResult.Failed("该菜单下还有子菜单，无法删除");
                }

                await _menuRepository.SoftDeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("菜单删除成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"删除菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取菜单详情
        /// </summary>
        public async Task<ApiResult<MenuDto>> GetMenuByIdAsync(long id)
        {
            try
            {
                var menu = await _menuRepository.GetByIdAsync(id);
                if (menu == null)
                {
                    return ApiResult<MenuDto>.Failed("菜单不存在");
                }

                var menuDto = menu.Adapt<MenuDto>();
                return ApiResult<MenuDto>.Succeed(menuDto);
            }
            catch (Exception ex)
            {
                return ApiResult<MenuDto>.Failed($"获取菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetMenuTreeAsync(MenuQueryDto? query = null)
        {
            try
            {
                var menus = await _menuRepository.GetMenuTreeAsync(
                    query?.MenuName, 
                    query?.Status);

                var menuTree = BuildMenuTree(menus);
                return ApiResult<List<MenuTreeDto>>.Succeed(menuTree);
            }
            catch (Exception ex)
            {
                return ApiResult<List<MenuTreeDto>>.Failed($"获取菜单树失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 根据用户ID获取用户菜单树
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetUserMenuTreeAsync(long userId)
        {
            try
            {
                var menus = await _menuRepository.GetMenusByUserIdAsync(userId);
                var menuTree = BuildMenuTree(menus);
                return ApiResult<List<MenuTreeDto>>.Succeed(menuTree);
            }
            catch (Exception ex)
            {
                return ApiResult<List<MenuTreeDto>>.Failed($"获取用户菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 根据用户名获取用户菜单树
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetUserMenuTreeByUsernameAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    return ApiResult<List<MenuTreeDto>>.Failed("用户不存在");
                }

                return await GetUserMenuTreeAsync(user.Id);
            }
            catch (Exception ex)
            {
                return ApiResult<List<MenuTreeDto>>.Failed($"获取用户菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 根据角色ID获取角色菜单
        /// </summary>
        public async Task<ApiResult<List<MenuDto>>> GetMenusByRoleIdAsync(long roleId)
        {
            try
            {
                var menus = await _menuRepository.GetMenusByRoleIdAsync(roleId);
                var menuDtos = menus.Adapt<List<MenuDto>>();
                return ApiResult<List<MenuDto>>.Succeed(menuDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<MenuDto>>.Failed($"获取角色菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 检查菜单编码是否存在
        /// </summary>
        public async Task<ApiResult<bool>> MenuCodeExistsAsync(string menuCode, long? excludeId = null)
        {
            try
            {
                var exists = await _menuRepository.IsMenuCodeExistsAsync(menuCode, excludeId);
                return ApiResult<bool>.Succeed(exists);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failed($"检查菜单编码失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取父菜单列表
        /// </summary>
        public async Task<ApiResult<List<MenuDto>>> GetParentMenusAsync()
        {
            try
            {
                var menus = await _menuRepository.GetParentMenusAsync();
                var menuDtos = menus.Adapt<List<MenuDto>>();
                return ApiResult<List<MenuDto>>.Succeed(menuDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<MenuDto>>.Failed($"获取父菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 根据父ID获取子菜单
        /// </summary>
        public async Task<ApiResult<List<MenuDto>>> GetChildMenusAsync(long parentId)
        {
            try
            {
                var menus = await _menuRepository.GetChildMenusAsync(parentId);
                var menuDtos = menus.Adapt<List<MenuDto>>();
                return ApiResult<List<MenuDto>>.Succeed(menuDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<MenuDto>>.Failed($"获取子菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新菜单排序
        /// </summary>
        public async Task<ApiResult> UpdateMenuSortAsync(long menuId, int sort)
        {
            try
            {
                await _menuRepository.UpdateMenuSortAsync(menuId, sort);
                await _unitOfWork.SaveChangesAsync();
                return ApiResult.Succeed("菜单排序更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"更新菜单排序失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新菜单状态
        /// </summary>
        public async Task<ApiResult> UpdateMenuStatusAsync(long id, bool status)
        {
            try
            {
                var menu = await _menuRepository.GetByIdAsync(id);
                if (menu == null)
                {
                    return ApiResult.Failed("菜单不存在");
                }

                menu.Status = status;
                await _menuRepository.UpdateAsync(menu);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("菜单状态更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"更新菜单状态失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取菜单选择树（用于角色分配菜单权限）
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetMenuTreeForRoleAssignAsync(long? roleId = null)
        {
            try
            {
                var allMenus = await _menuRepository.GetMenuTreeAsync();
                var menuTree = BuildMenuTree(allMenus);

                // 如果指定了角色ID，标记已选中的菜单
                if (roleId.HasValue)
                {
                    var roleMenus = await _menuRepository.GetMenusByRoleIdAsync(roleId.Value);
                    var roleMenuIds = roleMenus.Select(m => m.Id).ToHashSet();
                    MarkCheckedMenus(menuTree, roleMenuIds);
                }

                return ApiResult<List<MenuTreeDto>>.Succeed(menuTree);
            }
            catch (Exception ex)
            {
                return ApiResult<List<MenuTreeDto>>.Failed($"获取菜单选择树失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 保存角色菜单权限
        /// </summary>
        public async Task<ApiResult> SaveRoleMenuPermissionsAsync(long roleId, List<long> menuIds)
        {
            try
            {
                // 删除现有权限
                var roleMenuRepository = _unitOfWork.GetBaseRepository<RolePermission>();
                var existingRoleMenus = await roleMenuRepository.FindAsync(rm => rm.RoleId == roleId);

                if (existingRoleMenus.Any())
                {
                    await roleMenuRepository.DeleteRangeAsync(existingRoleMenus);
                }

                // 添加新权限
                var roleMenus = menuIds.Select(menuId => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = menuId,
                    CreateTime = DateTime.Now
                }).ToList();

                await roleMenuRepository.AddRangeAsync(roleMenus);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("角色菜单权限保存成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"保存角色菜单权限失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取展开的菜单Keys
        /// </summary>
        public async Task<ApiResult<List<string>>> GetExpandedKeysAsync()
        {
            try
            {
                var parentMenus = await _menuRepository.GetAllParentMenusAsync();
                var expandedKeys = parentMenus.Select(m => m.Id.ToString()).ToList();
                return ApiResult<List<string>>.Succeed(expandedKeys);
            }
            catch (Exception ex)
            {
                return ApiResult<List<string>>.Failed($"获取展开菜单失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取角色已选中的菜单Keys
        /// </summary>
        public async Task<ApiResult<List<string>>> GetCheckedKeysByRoleIdAsync(long roleId)
        {
            try
            {
                var roleMenus = await _menuRepository.GetMenusByRoleIdAsync(roleId);
                var checkedKeys = roleMenus.Select(m => m.Id.ToString()).ToList();
                return ApiResult<List<string>>.Succeed(checkedKeys);
            }
            catch (Exception ex)
            {
                return ApiResult<List<string>>.Failed($"获取角色菜单失败：{ex.Message}");
            }
        }

        #region 私有方法

        /// <summary>
        /// 设置菜单层级和路径信息
        /// </summary>
        private async Task SetMenuHierarchyInfo(Permission menu)
        {
            if (menu.ParentId.HasValue)
            {
                var parentMenu = await _menuRepository.GetByIdAsync(menu.ParentId.Value);
                if (parentMenu != null)
                {
                    menu.ParentIds = $"{parentMenu.ParentIds},[{menu.ParentId}]";
                    await _menuRepository.UpdateAsync(parentMenu);
                }
                else
                {
                    menu.ParentIds = $"[{menu.ParentId}]";
                }
            }
            else
            {
                menu.ParentIds = "[0]";
            }
        }

        /// <summary>
        /// 构建菜单树
        /// </summary>
        private List<MenuTreeDto> BuildMenuTree(IEnumerable<Permission> menus)
        {
            var menuList = menus.ToList();
            var menuDict = menuList.ToDictionary(m => m.Id, m => m.Adapt<MenuTreeDto>());

            var rootMenus = new List<MenuTreeDto>();

            foreach (var menu in menuDict.Values)
            {
                if (menu.ParentId.HasValue && menuDict.ContainsKey(menu.ParentId.Value))
                {
                    menuDict[menu.ParentId.Value].Children.Add(menu);
                }
                else
                {
                    rootMenus.Add(menu);
                }
            }

            // 排序
            SortMenuTree(rootMenus);
            return rootMenus;
        }

        /// <summary>
        /// 菜单树排序
        /// </summary>
        private void SortMenuTree(List<MenuTreeDto> menus)
        {
            menus.Sort((x, y) => x.Sort.CompareTo(y.Sort));
            foreach (var menu in menus)
            {
                if (menu.Children.Any())
                {
                    SortMenuTree(menu.Children);
                }
            }
        }

        /// <summary>
        /// 标记已选中的菜单
        /// </summary>
        private void MarkCheckedMenus(List<MenuTreeDto> menus, HashSet<long> checkedIds)
        {
            foreach (var menu in menus)
            {
                menu.Checked = checkedIds.Contains(menu.Id);
                if (menu.Children.Any())
                {
                    MarkCheckedMenus(menu.Children, checkedIds);
                }
            }
        }

        #endregion
    }
}
