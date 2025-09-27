using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Domain.Entities;
using ApiServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Infrastructure.Repositories
{
    /// <summary>
    /// 菜单仓储实现
    /// </summary>
    public class MenuRepository : BaseRepository<Menu>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据父菜单ID获取子菜单列表
        /// </summary>
        public async Task<IEnumerable<Menu>> GetByParentIdAsync(long? parentId)
        {
            var query = _dbSet.AsQueryable();
            
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
        /// 获取菜单树
        /// </summary>
        public async Task<IEnumerable<Menu>> GetMenuTreeAsync()
        {
            var allMenus = await _dbSet
                .Where(m => !m.IsDeleted && m.Status)
                .OrderBy(m => m.Sort)
                .ToListAsync();

            return BuildMenuTree(allMenus, null);
        }

        /// <summary>
        /// 根据角色ID获取菜单列表
        /// </summary>
        public async Task<IEnumerable<Menu>> GetByRoleIdAsync(long roleId)
        {
            return await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId)
                .Select(rm => rm.Menu)
                .Where(m => !m.IsDeleted && m.Status)
                .OrderBy(m => m.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 根据用户ID获取菜单列表
        /// </summary>
        public async Task<IEnumerable<Menu>> GetByUserIdAsync(long userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.RoleMenus, ur => ur.RoleId, rm => rm.RoleId, (ur, rm) => rm)
                .Select(rm => rm.Menu)
                .Where(m => !m.IsDeleted && m.Status)
                .Distinct()
                .OrderBy(m => m.Sort)
                .ToListAsync();
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
        public async Task<Menu?> GetByMenuCodeAsync(string menuCode)
        {
            return await _dbSet
                .Where(m => m.Code == menuCode && !m.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据用户ID获取菜单列表（重命名方法）
        /// </summary>
        public async Task<IEnumerable<Menu>> GetMenusByUserIdAsync(long userId)
        {
            return await GetByUserIdAsync(userId);
        }

        /// <summary>
        /// 根据角色ID获取菜单列表（重命名方法）
        /// </summary>
        public async Task<IEnumerable<Menu>> GetMenusByRoleIdAsync(long roleId)
        {
            return await GetByRoleIdAsync(roleId);
        }

        /// <summary>
        /// 获取菜单树（带过滤条件）
        /// </summary>
        public async Task<IEnumerable<Menu>> GetMenuTreeAsync(string? menuName = null, bool? status = null)
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
            return BuildMenuTree(allMenus, null);
        }

        /// <summary>
        /// 根据父ID获取子菜单
        /// </summary>
        public async Task<IEnumerable<Menu>> GetChildMenusAsync(long parentId)
        {
            return await GetByParentIdAsync(parentId);
        }

        /// <summary>
        /// 获取父菜单列表
        /// </summary>
        public async Task<IEnumerable<Menu>> GetParentMenusAsync()
        {
            return await GetByParentIdAsync(null);
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
        /// 获取所有父菜单（非叶子节点）
        /// </summary>
        public async Task<IEnumerable<Menu>> GetAllParentMenusAsync()
        {
            return await _dbSet
                .Where(m => !m.IsDeleted && !m.IsLeaf)
                .OrderBy(m => m.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 根据URL获取菜单
        /// </summary>
        public async Task<Menu?> GetByUrlAsync(string url)
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
        private IEnumerable<Menu> BuildMenuTree(IEnumerable<Menu> allMenus, long? parentId)
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
