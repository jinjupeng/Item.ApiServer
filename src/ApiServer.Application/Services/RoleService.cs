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
            try
            {
                // 检查角色名称是否已存在
                if (await _roleRepository.IsRoleNameExistsAsync(dto.RoleName))
                {
                    return ApiResult<long>.FailResult("角色名称已存在");
                }

                var role = dto.Adapt<Role>();
                await _roleRepository.AddAsync(role);
                await _unitOfWork.SaveChangesAsync();

                // 分配权限
                if (dto.MenuIds.Any())
                {
                    await _roleRepository.AssignMenusToRoleAsync(role.Id, dto.MenuIds);
                }

                if (dto.ApiIds.Any())
                {
                    await _roleRepository.AssignApisToRoleAsync(role.Id, dto.ApiIds);
                }

                await _unitOfWork.SaveChangesAsync();

                return ApiResult<long>.SuccessResult(role.Id, "角色创建成功");
            }
            catch (Exception ex)
            {
                return ApiResult<long>.FailResult($"创建角色失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        public async Task<ApiResult> UpdateRoleAsync(long id, UpdateRoleDto dto)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResult.FailResult("角色不存在");
                }

                // 检查角色名称是否已存在（排除当前角色）
                if (await _roleRepository.IsRoleNameExistsAsync(dto.RoleName, id))
                {
                    return ApiResult.FailResult("角色名称已存在");
                }

                // 更新角色信息
                dto.Adapt(role);
                await _roleRepository.UpdateAsync(role);

                // 重新分配权限
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

                return ApiResult.SuccessResult("角色更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.FailResult($"更新角色失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        public async Task<ApiResult> DeleteRoleAsync(long id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResult.FailResult("角色不存在");
                }

                // 检查是否有用户使用该角色
                var userCount = await _roleRepository.GetUserCountByRoleIdAsync(id);
                if (userCount > 0)
                {
                    return ApiResult.FailResult($"该角色下还有 {userCount} 个用户，无法删除");
                }

                await _roleRepository.SoftDeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.SuccessResult("角色删除成功");
            }
            catch (Exception ex)
            {
                return ApiResult.FailResult($"删除角色失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取角色详情
        /// </summary>
        public async Task<ApiResult<RoleDto>> GetRoleByIdAsync(long id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResult<RoleDto>.FailResult("角色不存在");
                }

                var roleDto = role.Adapt<RoleDto>();
                
                // 获取角色权限
                var menus = await _roleRepository.GetRoleMenusAsync(id);
                var apis = await _roleRepository.GetRoleApisAsync(id);
                
                roleDto.MenuIds = menus.Select(m => m.Id).ToList();
                roleDto.PermissionIds = apis.Select(a => a.Id).ToList();

                return ApiResult<RoleDto>.SuccessResult(roleDto);
            }
            catch (Exception ex)
            {
                return ApiResult<RoleDto>.FailResult($"获取角色失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 分页查询角色
        /// </summary>
        public async Task<ApiResult<PagedResult<RoleDto>>> GetPagedRolesAsync(RoleQueryDto query)
        {
            try
            {
                var (roles, total) = await _roleRepository.GetPagedRolesAsync(
                    query.PageIndex,
                    query.PageSize,
                    query.RoleName,
                    query.Status);

                var roleDtos = new List<RoleDto>();
                foreach (var role in roles)
                {
                    var roleDto = role.Adapt<RoleDto>();
                    roleDtos.Add(roleDto);
                }

                var pagedResult = new PagedResult<RoleDto>(
                    roleDtos,
                    total,
                    query.PageIndex,
                    query.PageSize);

                return ApiResult<PagedResult<RoleDto>>.SuccessResult(pagedResult);
            }
            catch (Exception ex)
            {
                return ApiResult<PagedResult<RoleDto>>.FailResult($"查询角色失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        public async Task<ApiResult<List<RoleDto>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                var roleDtos = roles.Adapt<List<RoleDto>>();

                return ApiResult<List<RoleDto>>.SuccessResult(roleDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<RoleDto>>.FailResult($"获取角色列表失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 根据用户ID获取角色列表
        /// </summary>
        public async Task<ApiResult<List<RoleDto>>> GetRolesByUserIdAsync(long userId)
        {
            try
            {
                var roles = await _roleRepository.GetRolesByUserIdAsync(userId);
                var roleDtos = roles.Adapt<List<RoleDto>>();

                return ApiResult<List<RoleDto>>.SuccessResult(roleDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<RoleDto>>.FailResult($"获取用户角色失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        public async Task<ApiResult> AssignRolesToUserAsync(UserRoleAssignDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    return ApiResult.FailResult("用户不存在");
                }

                // 删除用户现有角色
                var userRoleRepository = _unitOfWork.GetBaseRepository<UserRole>();
                var existingUserRoles = await userRoleRepository.FindAsync(ur => ur.UserId == dto.UserId);

                if (existingUserRoles.Any())
                {
                    await userRoleRepository.DeleteRangeAsync(existingUserRoles);
                }

                // 添加新角色
                var userRoles = dto.RoleIds.Select(roleId => new UserRole
                {
                    UserId = dto.UserId,
                    RoleId = roleId,
                    CreateTime = DateTime.Now
                }).ToList();

                await userRoleRepository.AddRangeAsync(userRoles);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.SuccessResult("角色分配成功");
            }
            catch (Exception ex)
            {
                return ApiResult.FailResult($"分配角色失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 移除用户的角色
        /// </summary>
        public async Task<ApiResult> RemoveRolesFromUserAsync(long userId, List<long> roleIds)
        {
            try
            {
                var userRoleRepository = _unitOfWork.GetBaseRepository<UserRole>();
                var userRoles = await userRoleRepository.FindAsync(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId));

                if (userRoles.Any())
                {
                    await userRoleRepository.DeleteRangeAsync(userRoles);
                    await _unitOfWork.SaveChangesAsync();
                }

                return ApiResult.SuccessResult("角色移除成功");
            }
            catch (Exception ex)
            {
                return ApiResult.FailResult($"移除角色失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 检查角色名称是否存在
        /// </summary>
        public async Task<ApiResult<bool>> RoleNameExistsAsync(string roleName, long? excludeId = null)
        {
            try
            {
                var exists = await _roleRepository.IsRoleNameExistsAsync(roleName, excludeId);
                return ApiResult<bool>.SuccessResult(exists);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.FailResult($"检查角色名称失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        public async Task<ApiResult> AssignPermissionsToRoleAsync(RolePermissionDto dto)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(dto.RoleId);
                if (role == null)
                {
                    return ApiResult.FailResult("角色不存在");
                }

                // 重新分配权限
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

                return ApiResult.SuccessResult("权限分配成功");
            }
            catch (Exception ex)
            {
                return ApiResult.FailResult($"分配权限失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        public async Task<ApiResult<RolePermissionDto>> GetRolePermissionsAsync(long roleId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResult<RolePermissionDto>.FailResult("角色不存在");
                }

                var menus = await _roleRepository.GetRoleMenusAsync(roleId);
                var apis = await _roleRepository.GetRoleApisAsync(roleId);

                var permissionDto = new RolePermissionDto
                {
                    RoleId = roleId,
                    MenuIds = menus.Select(m => m.Id).ToList(),
                    ApiIds = apis.Select(a => a.Id).ToList()
                };

                return ApiResult<RolePermissionDto>.SuccessResult(permissionDto);
            }
            catch (Exception ex)
            {
                return ApiResult<RolePermissionDto>.FailResult($"获取角色权限失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更新角色状态
        /// </summary>
        public async Task<ApiResult> UpdateRoleStatusAsync(long id, bool status)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResult.FailResult("角色不存在");
                }

                role.Status = status;
                await _roleRepository.UpdateAsync(role);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.SuccessResult("角色状态更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.FailResult($"更新角色状态失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取用户角色选择列表
        /// </summary>
        public async Task<ApiResult<List<RoleDto>>> GetRolesForUserAssignAsync(long userId)
        {
            try
            {
                var allRoles = await _roleRepository.GetAllAsync();
                var userRoles = await _roleRepository.GetRolesByUserIdAsync(userId);
                var userRoleIds = userRoles.Select(r => r.Id).ToHashSet();

                var roleDtos = allRoles.Select(role =>
                {
                    var roleDto = role.Adapt<RoleDto>();
                    // 可以添加是否已选中的标识
                    return roleDto;
                }).ToList();

                return ApiResult<List<RoleDto>>.SuccessResult(roleDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<RoleDto>>.FailResult($"获取角色选择列表失败：{ex.Message}");
            }
        }
    }
}
