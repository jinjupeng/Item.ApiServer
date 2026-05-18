using ApiServer.Application.DTOs.Role;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Domain.Entities;
using ApiServer.Shared.Common;
using Mapster;

namespace ApiServer.Application.Services
{
    /// <summary>
    /// 角色服务实现
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(
            IRoleRepository roleRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        public async Task<ApiResult<long>> CreateRoleAsync(CreateRoleDto dto)
        {
            if (await _roleRepository.IsRoleNameExistsAsync(dto.RoleName))
            {
                return ApiResultFactory.Conflict<long>("角色名称已存在");
            }

            var role = dto.Adapt<Role>();
            await _roleRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            if (dto.MenuIds.Any())
            {
                await _roleRepository.AssignMenusToRoleAsync(role.Id, dto.MenuIds);
            }

            if (dto.ApiIds.Any())
            {
                await _roleRepository.AssignApisToRoleAsync(role.Id, dto.ApiIds);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResult<long>.Succeed(role.Id, "角色创建成功");
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        public async Task<ApiResult> UpdateRoleAsync(long id, UpdateRoleDto dto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return ApiResultFactory.NotFound("角色不存在");
            }

            if (await _roleRepository.IsRoleNameExistsAsync(dto.RoleName, id))
            {
                return ApiResultFactory.Conflict("角色名称已存在");
            }

            dto.Adapt(role);
            await _roleRepository.UpdateAsync(role);

            await _roleRepository.RemoveRoleMenusAsync(id);
            await _roleRepository.RemoveRoleApisAsync(id);

            if (dto.MenuIds.Any())
            {
                await _roleRepository.AssignMenusToRoleAsync(id, dto.MenuIds);
            }

            if (dto.ApiIds.Any())
            {
                await _roleRepository.AssignApisToRoleAsync(id, dto.ApiIds);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("角色更新成功");
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        public async Task<ApiResult> DeleteRoleAsync(long id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return ApiResultFactory.NotFound("角色不存在");
            }

            var userCount = await _roleRepository.GetUserCountByRoleIdAsync(id);
            if (userCount > 0)
            {
                return ApiResultFactory.Conflict($"该角色下还有 {userCount} 个用户，无法删除");
            }

            await _roleRepository.SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("角色删除成功");
        }

        /// <summary>
        /// 获取角色详情
        /// </summary>
        public async Task<ApiResult<RoleDto>> GetRoleByIdAsync(long id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return ApiResultFactory.NotFound<RoleDto>("角色不存在");
            }

            var roleDto = role.Adapt<RoleDto>();
            var menus = await _roleRepository.GetRoleMenusAsync(id);
            var apis = await _roleRepository.GetRoleApisAsync(id);

            roleDto.MenuIds = menus.Select(m => m.Id).ToList();
            roleDto.PermissionIds = apis.Select(a => a.Id).ToList();

            return ApiResult<RoleDto>.Succeed(roleDto);
        }

        /// <summary>
        /// 分页查询角色
        /// </summary>
        public async Task<ApiResult<PagedResult<RoleDto>>> GetPagedRolesAsync(RoleQueryDto query)
        {
            var (roles, total) = await _roleRepository.GetPagedRolesAsync(
                query.PageIndex,
                query.PageSize,
                query.RoleName,
                query.Status);

            var roleDtos = roles.Select(role => role.Adapt<RoleDto>()).ToList();
            var pagedResult = new PagedResult<RoleDto>(
                roleDtos,
                total,
                query.PageIndex,
                query.PageSize);

            return ApiResult<PagedResult<RoleDto>>.Succeed(pagedResult);
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        public async Task<ApiResult<List<RoleDto>>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            var roleDtos = roles.Adapt<List<RoleDto>>();

            return ApiResult<List<RoleDto>>.Succeed(roleDtos);
        }

        /// <summary>
        /// 根据用户ID获取角色列表
        /// </summary>
        public async Task<ApiResult<List<RoleDto>>> GetRolesByUserIdAsync(long userId)
        {
            var roles = await _roleRepository.GetRolesByUserIdAsync(userId);
            var roleDtos = roles.Adapt<List<RoleDto>>();

            return ApiResult<List<RoleDto>>.Succeed(roleDtos);
        }

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        public async Task<ApiResult> AssignRolesToUserAsync(UserRoleAssignDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return ApiResultFactory.NotFound("用户不存在");
            }

            var userRoleRepository = _unitOfWork.GetBaseRepository<UserRole>();
            var existingUserRoles = await userRoleRepository.FindAsync(ur => ur.UserId == dto.UserId);

            if (existingUserRoles.Any())
            {
                await userRoleRepository.DeleteRangeAsync(existingUserRoles);
            }

            var userRoles = dto.RoleIds.Select(roleId => new UserRole
            {
                UserId = dto.UserId,
                RoleId = roleId,
                CreateTime = DateTime.Now
            }).ToList();

            await userRoleRepository.AddRangeAsync(userRoles);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("角色分配成功");
        }

        /// <summary>
        /// 移除用户的角色
        /// </summary>
        public async Task<ApiResult> RemoveRolesFromUserAsync(long userId, List<long> roleIds)
        {
            var userRoleRepository = _unitOfWork.GetBaseRepository<UserRole>();
            var userRoles = await userRoleRepository.FindAsync(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId));

            if (userRoles.Any())
            {
                await userRoleRepository.DeleteRangeAsync(userRoles);
                await _unitOfWork.SaveChangesAsync();
            }

            return ApiResult.Succeed("角色移除成功");
        }

        /// <summary>
        /// 检查角色名称是否存在
        /// </summary>
        public async Task<ApiResult<bool>> RoleNameExistsAsync(string roleName, long? excludeId = null)
        {
            var exists = await _roleRepository.IsRoleNameExistsAsync(roleName, excludeId);
            return ApiResult<bool>.Succeed(exists);
        }

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        public async Task<ApiResult> AssignPermissionsToRoleAsync(RolePermissionDto dto)
        {
            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null)
            {
                return ApiResultFactory.NotFound("角色不存在");
            }

            await _roleRepository.RemoveRoleMenusAsync(dto.RoleId);
            await _roleRepository.RemoveRoleApisAsync(dto.RoleId);

            if (dto.MenuIds.Any())
            {
                await _roleRepository.AssignMenusToRoleAsync(dto.RoleId, dto.MenuIds);
            }

            if (dto.ApiIds.Any())
            {
                await _roleRepository.AssignApisToRoleAsync(dto.RoleId, dto.ApiIds);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("权限分配成功");
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        public async Task<ApiResult<RolePermissionDto>> GetRolePermissionsAsync(long roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                return ApiResultFactory.NotFound<RolePermissionDto>("角色不存在");
            }

            var menus = await _roleRepository.GetRoleMenusAsync(roleId);
            var apis = await _roleRepository.GetRoleApisAsync(roleId);

            var permissionDto = new RolePermissionDto
            {
                RoleId = roleId,
                MenuIds = menus.Select(m => m.Id).ToList(),
                ApiIds = apis.Select(a => a.Id).ToList()
            };

            return ApiResult<RolePermissionDto>.Succeed(permissionDto);
        }

        /// <summary>
        /// 更新角色状态
        /// </summary>
        public async Task<ApiResult> UpdateRoleStatusAsync(long id, bool status)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return ApiResultFactory.NotFound("角色不存在");
            }

            role.Status = status;
            await _roleRepository.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return ApiResult.Succeed("角色状态更新成功");
        }

        /// <summary>
        /// 获取用户角色选择列表
        /// </summary>
        public async Task<ApiResult<List<RoleDto>>> GetRolesForUserAssignAsync(long userId)
        {
            var allRoles = await _roleRepository.GetAllAsync();
            var userRoles = await _roleRepository.GetRolesByUserIdAsync(userId);
            var userRoleIds = userRoles.Select(r => r.Id).ToHashSet();

            var roleDtos = allRoles.Select(role =>
            {
                var roleDto = role.Adapt<RoleDto>();
                return roleDto;
            }).ToList();

            return ApiResult<List<RoleDto>>.Succeed(roleDtos);
        }
    }
}
