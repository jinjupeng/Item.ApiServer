using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Domain.Entities;
using ApiServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiServer.Infrastructure.Repositories
{
    /// <summary>
    /// 用户仓储实现
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .Include(u => u.Organization)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// 根据邮箱获取用户
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .Include(u => u.Organization)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// 获取用户及其角色
        /// </summary>
        public async Task<User?> GetUserWithRolesAsync(long userId)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// 获取组织下的所有用户
        /// </summary>
        public async Task<IEnumerable<User>> GetUsersByOrgIdAsync(long orgId)
        {
            return await _dbSet
                .Where(u => !u.IsDeleted && u.OrgId == orgId)
                .Include(u => u.Organization)
                .ToListAsync();
        }

        /// <summary>
        /// 获取用户的菜单权限
        /// </summary>
        public async Task<IEnumerable<Menu>> GetUserMenusAsync(long userId)
        {
            return await _context.Menus
                .Where(m => !m.IsDeleted)
                .Where(m => m.RoleMenus.Any(rm => 
                    rm.Role.UserRoles.Any(ur => ur.UserId == userId)))
                .OrderBy(m => m.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 获取用户的API权限
        /// </summary>
        public async Task<IEnumerable<Permission>> GetUserApisAsync(long userId)
        {
            return await _context.Permissions
                .Where(a => !a.IsDeleted)
                .Where(a => a.RoleApis.Any(ra => 
                    ra.Role.UserRoles.Any(ur => ur.UserId == userId)))
                .ToListAsync();
        }

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        public async Task<bool> IsUsernameExistsAsync(string username, long? excludeUserId = null)
        {
            var query = _dbSet.Where(u => !u.IsDeleted && u.Username == username);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId);
            }
            
            return await query.AnyAsync();
        }

        /// <summary>
        /// 检查邮箱是否存在
        /// </summary>
        public async Task<bool> IsEmailExistsAsync(string email, long? excludeUserId = null)
        {
            var query = _dbSet.Where(u => !u.IsDeleted && u.Email == email);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId);
            }
            
            return await query.AnyAsync();
        }

        /// <summary>
        /// 分页查询用户（包含组织信息）
        /// </summary>
        public async Task<(IEnumerable<User> users, int total)> GetPagedUsersAsync(
            int page, 
            int pageSize, 
            string? keyword = null, 
            long? orgId = null, 
            int? status = null)
        {
            IQueryable<User> query = _dbSet.Where(u => !u.IsDeleted);

            // 关键字搜索
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u => 
                    u.Username.Contains(keyword) ||
                    (u.Nickname != null && u.Nickname.Contains(keyword)) ||
                    (u.Email != null && u.Email.Contains(keyword)) ||
                    (u.Phone != null && u.Phone.Contains(keyword)));
            }

            // 组织过滤
            if (orgId.HasValue)
            {
                query = query.Where(u => u.OrgId == orgId);
            }

            // 状态过滤
            if (status.HasValue)
            {
                query = query.Where(u => (int)u.Status == status);
            }

            // 添加Include在最后
            query = query.Include(u => u.Organization);

            var total = await query.CountAsync();
            var users = await query
                .OrderByDescending(u => u.CreateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, total);
        }

        /// <summary>
        /// 批量更新用户状态
        /// </summary>
        public async Task BatchUpdateStatusAsync(IEnumerable<long> userIds, Domain.Enums.UserStatus status)
        {
            var users = await _dbSet
                .Where(u => userIds.Contains(u.Id) && !u.IsDeleted)
                .ToListAsync();

            foreach (var user in users)
            {
                user.Status = status;
            }

            await UpdateRangeAsync(users);
        }

        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        public async Task<(int totalUsers, int activeUsers, int inactiveUsers, int lockedUsers)> GetUserStatisticsAsync()
        {
            var users = await _dbSet.Where(u => !u.IsDeleted).ToListAsync();
            
            var totalUsers = users.Count;
            var activeUsers = users.Count(u => u.Status == Domain.Enums.UserStatus.Active);
            var inactiveUsers = users.Count(u => u.Status == Domain.Enums.UserStatus.Inactive);
            var lockedUsers = users.Count(u => u.Status == Domain.Enums.UserStatus.Locked);

            return (totalUsers, activeUsers, inactiveUsers, lockedUsers);
        }
    }
}