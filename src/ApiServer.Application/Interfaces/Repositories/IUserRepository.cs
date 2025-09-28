using ApiServer.Domain.Entities;

namespace ApiServer.Application.Interfaces.Repositories
{
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// 根据邮箱获取用户
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// 获取用户及其角色
        /// </summary>
        Task<User?> GetUserWithRolesAsync(long userId);

        /// <summary>
        /// 获取组织下的所有用户
        /// </summary>
        Task<IEnumerable<User>> GetUsersByOrgIdAsync(long orgId);

        /// <summary>
        /// 获取用户的菜单权限
        /// </summary>
        Task<IEnumerable<Menu>> GetUserMenusAsync(long userId);

        /// <summary>
        /// 获取用户的API权限
        /// </summary>
        Task<IEnumerable<Permission>> GetUserPermissionsAsync(long userId);

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        Task<bool> IsUsernameExistsAsync(string username, long? excludeUserId = null);

        /// <summary>
        /// 检查邮箱是否存在
        /// </summary>
        Task<bool> IsEmailExistsAsync(string email, long? excludeUserId = null);

        /// <summary>
        /// 分页查询用户（包含组织信息）
        /// </summary>
        Task<(IEnumerable<User> users, int total)> GetPagedUsersAsync(
            int page, 
            int pageSize, 
            string? keyword = null, 
            long? orgId = null, 
            int? status = null);

        /// <summary>
        /// 批量更新用户状态
        /// </summary>
        Task BatchUpdateStatusAsync(IEnumerable<long> userIds, Domain.Enums.UserStatus status);

        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        Task<(int totalUsers, int activeUsers, int inactiveUsers, int lockedUsers)> GetUserStatisticsAsync();
    }
}