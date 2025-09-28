using ApiServer.Application.DTOs.User;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Domain.Entities;
using ApiServer.Domain.Enums;
using ApiServer.Shared.Common;
using Mapster;
using System.Security.Cryptography;
using System.Text;

namespace ApiServer.Application.Services
{
    /// <summary>
    /// 用户服务实现
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        public async Task<ApiResult<long>> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                // 检查用户名是否已存在
                if (await _userRepository.IsUsernameExistsAsync(dto.Username))
                {
                    return ApiResult<long>.Failed("用户名已存在");
                }

                // 检查邮箱是否已存在
                if (!string.IsNullOrEmpty(dto.Email) && await _userRepository.IsEmailExistsAsync(dto.Email))
                {
                    return ApiResult<long>.Failed("邮箱已存在");
                }

                var user = dto.Adapt<User>();
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.Status = UserStatus.Enabled;

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult<long>.Succeed(user.Id, "用户创建成功");
            }
            catch (Exception ex)
            {
                return ApiResult<long>.Failed($"创建用户失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public async Task<ApiResult> UpdateUserAsync(long id, UpdateUserDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResult.Failed("用户不存在");
                }

                // 检查用户名是否已存在（排除当前用户）
                if (await _userRepository.IsUsernameExistsAsync(dto.Username, id))
                {
                    return ApiResult.Failed("用户名已存在");
                }

                // 检查邮箱是否已存在（排除当前用户）
                if (!string.IsNullOrEmpty(dto.Email) && await _userRepository.IsEmailExistsAsync(dto.Email, id))
                {
                    return ApiResult.Failed("邮箱已存在");
                }

                // 更新用户信息
                dto.Adapt(user);
                
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("用户更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"更新用户失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public async Task<ApiResult> DeleteUserAsync(long id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResult.Failed("用户不存在");
                }

                await _userRepository.SoftDeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("用户删除成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"删除用户失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        public async Task<ApiResult<UserDto>> GetUserByIdAsync(long id)
        {
            try
            {
                var user = await _userRepository.GetUserWithRolesAsync(id);
                if (user == null)
                {
                    return ApiResult<UserDto>.Failed("用户不存在");
                }

                var userDto = user.Adapt<UserDto>();
                userDto.Roles = user.UserRoles.Select(x => new DTOs.Role.BaseRoleDto
                {
                    Id = x.Role.Id,
                    Name = x.Role.Name,
                    Code = x.Role.Code ?? ""
                }).ToList();
                return ApiResult<UserDto>.Succeed(userDto);
            }
            catch (Exception ex)
            {
                return ApiResult<UserDto>.Failed($"获取用户失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        public async Task<ApiResult<UserDto>> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    return ApiResult<UserDto>.Failed("用户不存在");
                }

                var userDto = user.Adapt<UserDto>();
                return ApiResult<UserDto>.Succeed(userDto);
            }
            catch (Exception ex)
            {
                return ApiResult<UserDto>.Failed($"获取用户失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 分页查询用户
        /// </summary>
        public async Task<ApiResult<PagedResult<UserDto>>> GetPagedUsersAsync(UserQueryDto query)
        {
            try
            {
                var (users, total) = await _userRepository.GetPagedUsersAsync(
                    query.PageIndex, 
                    query.PageSize, 
                    query.Keyword, 
                    query.OrgId, 
                    (int?)query.Status);

                var userDtos = users.Adapt<List<UserDto>>();
                
                var pagedResult = new PagedResult<UserDto>(
                    userDtos,
                    total,
                    query.PageIndex,
                    query.PageSize);

                return ApiResult<PagedResult<UserDto>>.Succeed(pagedResult);
            }
            catch (Exception ex)
            {
                return ApiResult<PagedResult<UserDto>>.Failed($"查询用户失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 验证用户密码
        /// </summary>
        public async Task<ApiResult<bool>> ValidatePasswordAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    return ApiResult<bool>.Succeed(false, "用户不存在");
                }

                var isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                return ApiResult<bool>.Succeed(isValid);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failed($"验证密码失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        public async Task<ApiResult> UpdateUserStatusAsync(long id, UserStatus status)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResult.Failed("用户不存在");
                }

                user.Status = status;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("用户状态更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"更新用户状态失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        public async Task<ApiResult> ResetPasswordAsync(long id, ResetPasswordDto resetPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return ApiResult.Failed("用户不存在");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword);
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("密码重置成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"重置密码失败：{ex.Message}");
            }
        }


        #region 私有方法

        /// <summary>
        /// 哈希密码
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt"));
            return Convert.ToBase64String(hashedBytes);
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        private static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        #endregion
    }

}
