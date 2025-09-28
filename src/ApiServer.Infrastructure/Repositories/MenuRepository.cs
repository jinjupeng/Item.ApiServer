using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Domain.Entities;
using ApiServer.Domain.Enums;
using ApiServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Infrastructure.Repositories
{
    /// <summary>
    /// 菜单仓储实现
    /// </summary>
    public class MenuRepository : BaseRepository<Permission>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据父菜单ID获取子菜单列表（不包含按钮类型）
        /// </summary>
        public async Task<IEnumerable<Permission>> GetByParentIdAsync(long? parentId)
        {
            var query = _dbSet.AsQueryable().Where(m => m.Type != PermissionType.Button);
            
            if (parentId.HasValue)
            {
                query = query.Where(m => m.ParentId == parentId.Value);
            }
            else
            {
                query = query.Where(m => m.ParentId == null);
            }

            return await query
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 获取菜单树（包含所有类型，供管理端使用）
        /// </summary>
        public async Task<IEnumerable<Permission>> GetMenuTreeAsync()
        {
            var allMenus = await _dbSet
                .Where(m => !m.IsDeleted && m.Status)
                .OrderBy(m => m.Sort)
                .ToListAsync();

            return BuildMenuTree(allMenus, null);
        }

        /// <summary>
        /// 检查菜单编码是否存在
        /// </summary>
        public async Task<bool> IsMenuCodeExistsAsync(string menuCode, long? excludeId = null)
        {
            var query = _dbSet.Where(m => m.Code == menuCode && !m.IsDeleted);
            
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// 根据菜单编码获取菜单
        /// </summary>
        public async Task<Permission?> GetByMenuCodeAsync(string menuCode)
        {
            return await _dbSet
                .Where(m => m.Code == menuCode && !m.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据用户ID获取菜单列表（用于导航，排除按钮类型）
        /// </summary>
        public async Task<IEnumerable<Permission>> GetMenusByUserIdAsync(long userId)
        {
            // 查询用户所有角色关联的所有权限ID
            var permissionIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp.PermissionId)
                .Distinct()
                .ToListAsync();

            // 根据权限ID查询权限实体，并排除按钮类型
            return await _dbSet
                .Where(p => permissionIds.Contains(p.Id) && !p.IsDeleted && p.Status && p.Type != PermissionType.Button)
                .OrderBy(p => p.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 根据角色ID获取菜单列表
        /// </summary>
        public async Task<IEnumerable<Permission>> GetMenusByRoleIdAsync(long roleId)
        {
            return await _context.RolePermissions
                .Where(rm => rm.RoleId == roleId)
                .Select(rm => rm.Permission)
                .Where(m => !m.IsDeleted && m.Status)
                .OrderBy(m => m.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 获取菜单树（带过滤条件）
        /// </summary>
        public async Task<IEnumerable<Permission>> GetMenuTreeAsync(string? menuName = null, bool? status = null)
        {
            var query = _dbSet.Where(m => !m.IsDeleted);
            
            if (!string.IsNullOrEmpty(menuName))
            {
                query = query.Where(m => m.Name.Contains(menuName));
            }
            
            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            var allMenus = await query.OrderBy(m => m.Sort).ToListAsync();
            return allMenus;
        }

        /// <summary>
        /// 根据父ID获取子菜单
        /// </summary>
        public async Task<IEnumerable<Permission>> GetChildMenusAsync(long parentId)
        {
            return await GetByParentIdAsync(parentId);
        }

        /// <summary>
        /// 获取父菜单列表（作为可选父节点，排除按钮类型），返回树形结构
        /// </summary>
        public async Task<IEnumerable<Permission>> GetParentMenusAsync()
        {
            var allMenus = await _dbSet
                .Where(m => !m.IsDeleted && m.Type != PermissionType.Button)
                .OrderBy(m => m.Sort)
                .ToListAsync();

            return BuildMenuTree(allMenus, null);
        }

        /// <summary>
        /// 更新菜单排序
        /// </summary>
        public async Task UpdateMenuSortAsync(long menuId, int sort)
        {
            var menu = await GetByIdAsync(menuId);
            if (menu != null)
            {
                menu.Sort = sort;
                await UpdateAsync(menu);
            }
        }

        /// <summary>
        /// 获取最大排序值
        /// </summary>
        public async Task<int> GetMaxSortAsync(long? parentId = null)
        {
            var query = _dbSet.Where(m => !m.IsDeleted);
            
            if (parentId.HasValue)
            {
                query = query.Where(m => m.ParentId == parentId.Value);
            }
            else
            {
                query = query.Where(m => m.ParentId == null);
            }

            return await query.MaxAsync(m => (int?)m.Sort) ?? 0;
        }

        /// <summary>
        /// 检查是否有子菜单
        /// </summary>
        public async Task<bool> HasChildMenusAsync(long menuId)
        {
            return await _dbSet.AnyAsync(m => m.ParentId == menuId && !m.IsDeleted);
        }

        /// <summary>
        /// 获取所有父菜单（存在子节点的非按钮类型）
        /// </summary>
        public async Task<IEnumerable<Permission>> GetAllParentMenusAsync()
        {
            return await _dbSet
                .Where(m => !m.IsDeleted && m.Type != PermissionType.Button)
                .Where(m => _dbSet.Any(c => !c.IsDeleted && c.ParentId == m.Id))
                .OrderBy(m => m.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 根据URL获取菜单
        /// </summary>
        public async Task<Permission?> GetByUrlAsync(string url)
        {
            return await _dbSet
                .Where(m => m.Url == url && !m.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 批量更新菜单状态
        /// </summary>
        public async Task BatchUpdateStatusAsync(IEnumerable<long> menuIds, bool status)
        {
            var menus = await _dbSet
                .Where(m => menuIds.Contains(m.Id))
                .ToListAsync();

            foreach (var menu in menus)
            {
                menu.Status = status;
            }

            await UpdateRangeAsync(menus);
        }

        /// <summary>
        /// 构建菜单树
        /// </summary>
        private IEnumerable<Permission> BuildMenuTree(IEnumerable<Permission> allMenus, long? parentId)
        {
            return allMenus
                .Where(m => m.ParentId == parentId)
                .Select(m =>
                {
                    m.Children = BuildMenuTree(allMenus, m.Id).ToList();
                    return m;
                });
        }
    }
}
