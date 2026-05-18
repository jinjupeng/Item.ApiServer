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
        private readonly ICurrentUser _currentUser;

        public MenuService(
            IMenuRepository menuRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        public async Task<ApiResult<long>> CreateMenuAsync(CreateMenuDto dto)
        {
            if (!string.IsNullOrEmpty(dto.MenuCode) && await _menuRepository.IsMenuCodeExistsAsync(dto.MenuCode))
            {
                return ApiResultFactory.Conflict<long>("菜单编码已存在");
            }

            var menu = dto.Adapt<Permission>();
            await SetMenuHierarchyInfo(menu);

            await _menuRepository.AddAsync(menu);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult<long>.Succeed(menu.Id, "菜单创建成功");
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        public async Task<ApiResult> UpdateMenuAsync(long id, UpdateMenuDto dto)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
            {
                return ApiResultFactory.NotFound("菜单不存在");
            }

            if (!string.IsNullOrEmpty(dto.MenuCode) && await _menuRepository.IsMenuCodeExistsAsync(dto.MenuCode, id))
            {
                return ApiResultFactory.Conflict("菜单编码已存在");
            }

            dto.Adapt(menu);
            await SetMenuHierarchyInfo(menu);

            await _menuRepository.UpdateAsync(menu);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("菜单更新成功");
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        public async Task<ApiResult> DeleteMenuAsync(long id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
            {
                return ApiResultFactory.NotFound("菜单不存在");
            }

            if (await _menuRepository.HasChildMenusAsync(id))
            {
                return ApiResultFactory.Conflict("该菜单下还有子菜单，无法删除");
            }

            await _menuRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("菜单删除成功");
        }

        /// <summary>
        /// 获取菜单详情
        /// </summary>
        public async Task<ApiResult<MenuDto>> GetMenuByIdAsync(long id)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
            {
                return ApiResultFactory.NotFound<MenuDto>("菜单不存在");
            }

            var menuDto = menu.Adapt<MenuDto>();
            return ApiResult<MenuDto>.Succeed(menuDto);
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetMenuTreeAsync(MenuQueryDto? query = null)
        {
            var menus = await _menuRepository.GetMenuTreeAsync(
                query?.MenuName,
                query?.Status);

            var menuTree = BuildMenuTree(menus);
            return ApiResult<List<MenuTreeDto>>.Succeed(menuTree);
        }

        /// <summary>
        /// 根据用户ID获取用户菜单树
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetUserMenuTreeAsync(long userId)
        {
            var menus = await _menuRepository.GetMenusByUserIdAsync(userId);

            menus = menus
                .Where(m => m.Status)
                .Where(m => m.Type != Domain.Enums.PermissionType.Button)
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            var menuTree = BuildMenuTree(menus);
            return ApiResult<List<MenuTreeDto>>.Succeed(menuTree);
        }

        /// <summary>
        /// 根据用户名获取用户菜单树
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetUserMenuTreeByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return ApiResultFactory.NotFound<List<MenuTreeDto>>("用户不存在");
            }

            return await GetUserMenuTreeAsync(user.Id);
        }

        /// <summary>
        /// 根据角色ID获取角色菜单
        /// </summary>
        public async Task<ApiResult<List<MenuDto>>> GetMenusByRoleIdAsync(long roleId)
        {
            var menus = await _menuRepository.GetMenusByRoleIdAsync(roleId);
            var menuDtos = menus.Adapt<List<MenuDto>>();
            return ApiResult<List<MenuDto>>.Succeed(menuDtos);
        }

        /// <summary>
        /// 检查菜单编码是否存在
        /// </summary>
        public async Task<ApiResult<bool>> MenuCodeExistsAsync(string menuCode, long? excludeId = null)
        {
            var exists = await _menuRepository.IsMenuCodeExistsAsync(menuCode, excludeId);
            return ApiResult<bool>.Succeed(exists);
        }

        /// <summary>
        /// 获取父菜单列表
        /// </summary>
        public async Task<ApiResult<List<MenuDto>>> GetParentMenusAsync()
        {
            var menus = await _menuRepository.GetParentMenusAsync();
            var menuDtos = menus.Adapt<List<MenuDto>>();
            return ApiResult<List<MenuDto>>.Succeed(menuDtos);
        }

        /// <summary>
        /// 根据父ID获取子菜单
        /// </summary>
        public async Task<ApiResult<List<MenuDto>>> GetChildMenusAsync(long parentId)
        {
            var menus = await _menuRepository.GetChildMenusAsync(parentId);
            var menuDtos = menus.Adapt<List<MenuDto>>();
            return ApiResult<List<MenuDto>>.Succeed(menuDtos);
        }

        /// <summary>
        /// 更新菜单排序
        /// </summary>
        public async Task<ApiResult> UpdateMenuSortAsync(long menuId, int sort)
        {
            await _menuRepository.UpdateMenuSortAsync(menuId, sort);
            await _unitOfWork.SaveChangesAsync();
            return ApiResult.Succeed("菜单排序更新成功");
        }

        /// <summary>
        /// 更新菜单状态
        /// </summary>
        public async Task<ApiResult> UpdateMenuStatusAsync(long id, bool status)
        {
            var menu = await _menuRepository.GetByIdAsync(id);
            if (menu == null)
            {
                return ApiResultFactory.NotFound("菜单不存在");
            }

            menu.Status = status;
            await _menuRepository.UpdateAsync(menu);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("菜单状态更新成功");
        }

        /// <summary>
        /// 获取菜单选择树（用于角色分配菜单权限）
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetMenuTreeForRoleAssignAsync(long? roleId = null)
        {
            var allMenus = await _menuRepository.GetMenuTreeAsync();
            var menuTree = BuildMenuTree(allMenus);

            if (roleId.HasValue)
            {
                var roleMenus = await _menuRepository.GetMenusByRoleIdAsync(roleId.Value);
                var roleMenuIds = roleMenus.Select(m => m.Id).ToHashSet();
                MarkCheckedMenus(menuTree, roleMenuIds);
            }

            return ApiResult<List<MenuTreeDto>>.Succeed(menuTree);
        }

        /// <summary>
        /// 保存角色菜单权限
        /// </summary>
        public async Task<ApiResult> SaveRoleMenuPermissionsAsync(long roleId, List<long> menuIds)
        {
            var roleMenuRepository = _unitOfWork.GetBaseRepository<RolePermission>();
            var existingRoleMenus = await roleMenuRepository.FindAsync(rm => rm.RoleId == roleId);

            if (existingRoleMenus.Any())
            {
                await roleMenuRepository.DeleteRangeAsync(existingRoleMenus);
            }

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

        /// <summary>
        /// 获取展开的菜单Keys
        /// </summary>
        public async Task<ApiResult<List<string>>> GetExpandedKeysAsync()
        {
            var parentMenus = await _menuRepository.GetAllParentMenusAsync();
            var expandedKeys = parentMenus.Select(m => m.Id.ToString()).ToList();
            return ApiResult<List<string>>.Succeed(expandedKeys);
        }

        /// <summary>
        /// 获取角色已选中的菜单Keys
        /// </summary>
        public async Task<ApiResult<List<string>>> GetCheckedKeysByRoleIdAsync(long roleId)
        {
            var roleMenus = await _menuRepository.GetMenusByRoleIdAsync(roleId);
            var checkedKeys = roleMenus.Select(m => m.Id.ToString()).ToList();
            return ApiResult<List<string>>.Succeed(checkedKeys);
        }

        /// <summary>
        /// 获取当前用户菜单树
        /// </summary>
        public async Task<ApiResult<List<MenuTreeDto>>> GetCurrentUserMenuTreeAsync()
        {
            if (_currentUser.UserId == null)
            {
                return ApiResultFactory.Unauthorized<List<MenuTreeDto>>("无法获取当前用户信息");
            }

            return await GetUserMenuTreeAsync(_currentUser.UserId.Value);
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
            var menuDtos = menus.Adapt<List<MenuTreeDto>>();
            foreach (var menu in menuDtos)
            {
                menu.Children = new List<MenuTreeDto>();
            }

            var menuMap = menuDtos.ToDictionary(m => m.Id);
            var rootMenus = new List<MenuTreeDto>();

            foreach (var menu in menuDtos)
            {
                if (menu.ParentId.HasValue && menuMap.TryGetValue(menu.ParentId.Value, out var parent))
                {
                    if (!parent.Children.Any(child => child.Id == menu.Id))
                    {
                        parent.Children.Add(menu);
                    }
                }
                else
                {
                    rootMenus.Add(menu);
                }
            }

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
