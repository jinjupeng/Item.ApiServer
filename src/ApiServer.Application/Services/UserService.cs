using ApiServer.Application.DTOs.User;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Domain.Entities;
using ApiServer.Domain.Enums;
using ApiServer.Shared.Common;
using Mapster;

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
            if (await _userRepository.IsUsernameExistsAsync(dto.Username))
            {
                return ApiResultFactory.Conflict<long>("用户名已存在");
            }

            if (!string.IsNullOrEmpty(dto.Email) && await _userRepository.IsEmailExistsAsync(dto.Email))
            {
                return ApiResultFactory.Conflict<long>("邮箱已存在");
            }

            var user = dto.Adapt<User>();
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Status = UserStatus.Enabled;

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult<long>.Succeed(user.Id, "用户创建成功");
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public async Task<ApiResult> UpdateUserAsync(long id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResultFactory.NotFound("用户不存在");
            }

            if (await _userRepository.IsUsernameExistsAsync(dto.Username, id))
            {
                return ApiResultFactory.Conflict("用户名已存在");
            }

            if (!string.IsNullOrEmpty(dto.Email) && await _userRepository.IsEmailExistsAsync(dto.Email, id))
            {
                return ApiResultFactory.Conflict("邮箱已存在");
            }

            dto.Adapt(user);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("用户更新成功");
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public async Task<ApiResult> DeleteUserAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResultFactory.NotFound("用户不存在");
            }

            await _userRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("用户删除成功");
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        public async Task<ApiResult<UserDto>> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetUserWithRolesAsync(id);
            if (user == null)
            {
                return ApiResultFactory.NotFound<UserDto>("用户不存在");
            }

            var userDto = user.Adapt<UserDto>();
            userDto.Roles = user.UserRoles.Select(x => new DTOs.Role.BaseRoleDto
            {
                Id = x.Role.Id,
                Name = x.Role.Name,
                Code = x.Role.Code ?? string.Empty
            }).ToList();

            return ApiResult<UserDto>.Succeed(userDto);
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        public async Task<ApiResult<UserDto>> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return ApiResultFactory.NotFound<UserDto>("用户不存在");
            }

            var userDto = user.Adapt<UserDto>();
            return ApiResult<UserDto>.Succeed(userDto);
        }

        /// <summary>
        /// 分页查询用户
        /// </summary>
        public async Task<ApiResult<PagedResult<UserDto>>> GetPagedUsersAsync(UserQueryDto query)
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

        /// <summary>
        /// 验证用户密码
        /// </summary>
        public async Task<ApiResult<bool>> ValidatePasswordAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return ApiResultFactory.NotFound<bool>("用户不存在");
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return ApiResult<bool>.Succeed(isValid);
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        public async Task<ApiResult> UpdateUserStatusAsync(long id, UserStatus status)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResultFactory.NotFound("用户不存在");
            }

            user.Status = status;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("用户状态更新成功");
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        public async Task<ApiResult> ResetPasswordAsync(long id, ResetPasswordDto resetPassword)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ApiResultFactory.NotFound("用户不存在");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("密码重置成功");
        }
    }
}
