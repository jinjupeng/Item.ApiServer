using ApiServer.Application.DTOs.User;
using ApiServer.Domain.Enums;
using ApiServer.Shared.Common;

namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 创建用户
        /// </summary>
        Task<ApiResult<long>> CreateUserAsync(CreateUserDto dto);

        /// <summary>
        /// 更新用户
        /// </summary>
        Task<ApiResult> UpdateUserAsync(long id, UpdateUserDto dto);

        /// <summary>
        /// 删除用户
        /// </summary>
        Task<ApiResult> DeleteUserAsync(long id);

        /// <summary>
        /// 获取用户详情
        /// </summary>
        Task<ApiResult<UserDto>> GetUserByIdAsync(long id);

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        Task<ApiResult<UserDto>> GetUserByUsernameAsync(string username);

        /// <summary>
        /// 分页查询用户
        /// </summary>
        Task<ApiResult<PagedResult<UserDto>>> GetPagedUsersAsync(UserQueryDto query);

        /// <summary>
        /// 验证用户密码
        /// </summary>
        Task<ApiResult<bool>> ValidatePasswordAsync(string username, string password);

        /// <summary>
        /// 更新用户状态
        /// </summary>
        Task<ApiResult> UpdateUserStatusAsync(long id, UserStatus status);

        /// <summary>
        /// 重置用户密码
        /// </summary>
        Task<ApiResult> ResetPasswordAsync(long id, ResetPasswordDto newPassword);
    }
}