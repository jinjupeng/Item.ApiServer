using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Domain.Common;
using ApiServer.Domain.Entities;
using ApiServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Infrastructure.Repositories
{
    /// <summary>
    /// 角色仓储实现
    /// </summary>
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        public async Task<Role?> GetByRoleNameAsync(string roleName)
        {
            return await GetQueryable()
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        /// <summary>
        /// 根据用户ID获取角色列表
        /// </summary>
        public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(long userId)
        {
            return await _context.Roles
                .Where(r => !((SoftDeleteEntity)(object)r).IsDeleted)
                .Where(r => r.UserRoles.Any(ur => ur.UserId == userId))
                .ToListAsync();
        }

        /// <summary>
        /// 检查角色名称是否存在
        /// </summary>
        public async Task<bool> IsRoleNameExistsAsync(string roleName, long? excludeRoleId = null)
        {
            var query = GetQueryable().Where(r => r.RoleName == roleName);
            
            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId);
            }
            
            return await query.AnyAsync();
        }

        /// <summary>
        /// 分页查询角色
        /// </summary>
        public async Task<(IEnumerable<Role> roles, int total)> GetPagedRolesAsync(
            int page, 
            int pageSize, 
            string? roleName = null, 
            bool? status = null)
        {
            var query = GetQueryable();

            // 角色名称过滤
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                query = query.Where(r => r.RoleName.Contains(roleName));
            }

            // 状态过滤
            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status);
            }

            var total = await query.CountAsync();
            var roles = await query
                .OrderByDescending(r => r.CreateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (roles, total);
        }

        /// <summary>
        /// 获取角色的菜单权限
        /// </summary>
        public async Task<IEnumerable<Menu>> GetRoleMenusAsync(long roleId)
        {
            return await _context.Menus
                .Where(m => !((SoftDeleteEntity)(object)m).IsDeleted)
                .Where(m => m.RoleMenus.Any(rm => rm.RoleId == roleId))
                .OrderBy(m => m.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 获取角色的API权限
        /// </summary>
        public async Task<IEnumerable<Permission>> GetRoleApisAsync(long roleId)
        {
            return await _context.Permissions
                .Where(a => !((SoftDeleteEntity)(object)a).IsDeleted)
                .Where(a => a.RoleApis.Any(ra => ra.RoleId == roleId))
                .ToListAsync();
        }

        /// <summary>
        /// 为角色分配菜单权限
        /// </summary>
        public async Task AssignMenusToRoleAsync(long roleId, IEnumerable<long> menuIds)
        {
            var roleMenus = menuIds.Select(menuId => new RoleMenu
            {
                RoleId = roleId,
                MenuId = menuId,
                CreateTime = DateTime.Now
            });

            await _context.RoleMenus.AddRangeAsync(roleMenus);
        }

        /// <summary>
        /// 为角色分配API权限
        /// </summary>
        public async Task AssignApisToRoleAsync(long roleId, IEnumerable<long> apiIds)
        {
            var roleApis = apiIds.Select(apiId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = apiId,
                CreateTime = DateTime.Now
            });

            await _context.RolePermissions.AddRangeAsync(roleApis);
        }

        /// <summary>
        /// 移除角色的菜单权限
        /// </summary>
        public async Task RemoveRoleMenusAsync(long roleId)
        {
            var roleMenus = await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId)
                .ToListAsync();

            _context.RoleMenus.RemoveRange(roleMenus);
        }

        /// <summary>
        /// 移除角色的API权限
        /// </summary>
        public async Task RemoveRoleApisAsync(long roleId)
        {
            var roleApis = await _context.RolePermissions
                .Where(ra => ra.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(roleApis);
        }

        /// <summary>
        /// 获取角色下的用户数量
        /// </summary>
        public async Task<int> GetUserCountByRoleIdAsync(long roleId)
        {
            return await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .CountAsync();
        }

        /// <summary>
        /// 批量更新角色状态
        /// </summary>
        public async Task BatchUpdateStatusAsync(IEnumerable<long> roleIds, bool status)
        {
            var roles = await _dbSet
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync();

            foreach (var role in roles)
            {
                role.Status = status;
            }

            _context.Roles.UpdateRange(roles);
        }
    }
}
