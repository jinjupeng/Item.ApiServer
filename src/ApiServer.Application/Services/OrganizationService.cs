using ApiServer.Application.DTOs.Organization;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Domain.Entities;
using ApiServer.Shared.Common;
using Mapster;

namespace ApiServer.Application.Services
{
    /// <summary>
    /// 组织服务实现
    /// </summary>
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrganizationService(
            IOrganizationRepository organizationRepository,
            IUnitOfWork unitOfWork)
        {
            _organizationRepository = organizationRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 创建组织
        /// </summary>
        public async Task<ApiResult<long>> CreateOrganizationAsync(CreateOrganizationDto dto)
        {
            try
            {
                // 检查组织编码是否已存在
                if (!string.IsNullOrEmpty(dto.OrgCode))
                {
                    var codeExists = await _organizationRepository.IsOrgCodeExistsAsync(dto.OrgCode);
                    if (codeExists)
                    {
                        return ApiResult<long>.Failed("组织编码已存在");
                    }
                }

                // 验证父组织是否存在
                if (dto.OrgPid.HasValue)
                {
                    var parent = await _organizationRepository.GetByIdAsync(dto.OrgPid.Value);
                    if (parent == null)
                    {
                        return ApiResult<long>.Failed("父组织不存在");
                    }
                }

                var organization = dto.Adapt<Organization>();
                
                // 设置组织路径
                await SetOrganizationHierarchyAsync(organization);

                var result = await _organizationRepository.AddAsync(organization);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult<long>.Succeed(result.Id, "组织创建成功");
            }
            catch (Exception ex)
            {
                return ApiResult<long>.Failed($"创建组织失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新组织
        /// </summary>
        public async Task<ApiResult> UpdateOrganizationAsync(long id, UpdateOrganizationDto dto)
        {
            try
            {
                var organization = await _organizationRepository.GetByIdAsync(id);
                if (organization == null)
                {
                    return ApiResult.Failed("组织不存在");
                }

                // 检查组织编码是否已存在
                if (!string.IsNullOrEmpty(dto.OrgCode))
                {
                    var codeExists = await _organizationRepository.IsOrgCodeExistsAsync(dto.OrgCode, id);
                    if (codeExists)
                    {
                        return ApiResult.Failed("组织编码已存在");
                    }
                }

                // 验证父组织是否存在
                if (dto.OrgPid.HasValue)
                {
                    var parent = await _organizationRepository.GetByIdAsync(dto.OrgPid.Value);
                    if (parent == null)
                    {
                        return ApiResult.Failed("父组织不存在");
                    }

                    // 不能将自己设为父组织
                    if (dto.OrgPid.Value == id)
                    {
                        return ApiResult.Failed("不能将自己设为父组织");
                    }
                }

                dto.Adapt(organization);
                
                // 重新设置组织路径
                await SetOrganizationHierarchyAsync(organization);

                await _organizationRepository.UpdateAsync(organization);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("组织更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"更新组织失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        public async Task<ApiResult> DeleteOrganizationAsync(long id)
        {
            try
            {
                var organization = await _organizationRepository.GetByIdAsync(id);
                if (organization == null)
                {
                    return ApiResult.Failed("组织不存在");
                }

                // 检查是否有子组织
                var children = await _organizationRepository.GetChildOrganizationsAsync(id);
                if (children.Any())
                {
                    return ApiResult.Failed("存在子组织，无法删除");
                }

                await _organizationRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("组织删除成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"删除组织失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取组织详情
        /// </summary>
        public async Task<ApiResult<OrganizationDto>> GetOrganizationByIdAsync(long id)
        {
            try
            {
                var organization = await _organizationRepository.GetByIdAsync(id);
                if (organization == null)
                {
                    return ApiResult<OrganizationDto>.Failed("组织不存在");
                }

                var dto = organization.Adapt<OrganizationDto>();
                return ApiResult<OrganizationDto>.Succeed(dto);
            }
            catch (Exception ex)
            {
                return ApiResult<OrganizationDto>.Failed($"获取组织详情失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取组织树
        /// </summary>
        public async Task<ApiResult<List<OrganizationTreeDto>>> GetOrganizationTreeAsync(OrganizationQueryDto? query = null)
        {
            try
            {
                var organizations = await _organizationRepository.GetOrganizationTreeAsync();
                var treeDtos = organizations.Adapt<List<OrganizationTreeDto>>();
                
                return ApiResult<List<OrganizationTreeDto>>.Succeed(treeDtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<OrganizationTreeDto>>.Failed($"获取组织树失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 根据父ID获取子组织
        /// </summary>
        public async Task<ApiResult<List<OrganizationDto>>> GetChildOrganizationsAsync(long parentId)
        {
            try
            {
                var organizations = await _organizationRepository.GetChildOrganizationsAsync(parentId);
                var dtos = organizations.Adapt<List<OrganizationDto>>();
                
                return ApiResult<List<OrganizationDto>>.Succeed(dtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<OrganizationDto>>.Failed($"获取子组织失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查组织编码是否存在
        /// </summary>
        public async Task<ApiResult<bool>> OrgCodeExistsAsync(string orgCode, long? excludeId = null)
        {
            try
            {
                var exists = await _organizationRepository.IsOrgCodeExistsAsync(orgCode, excludeId);
                return ApiResult<bool>.Succeed(exists);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failed($"检查组织编码失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取父组织列表
        /// </summary>
        public async Task<ApiResult<List<OrganizationDto>>> GetParentOrganizationsAsync()
        {
            try
            {
                var organizations = await _organizationRepository.GetParentOrganizationsAsync();
                var dtos = organizations.Adapt<List<OrganizationDto>>();
                
                return ApiResult<List<OrganizationDto>>.Succeed(dtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<OrganizationDto>>.Failed($"获取父组织列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新组织排序
        /// </summary>
        public async Task<ApiResult> UpdateOrganizationSortAsync(long orgId, int sort)
        {
            try
            {
                var organization = await _organizationRepository.GetByIdAsync(orgId);
                if (organization == null)
                {
                    return ApiResult.Failed("组织不存在");
                }

                organization.Sort = sort;
                await _organizationRepository.UpdateAsync(organization);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("排序更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"更新排序失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取组织下的用户数量
        /// </summary>
        public async Task<ApiResult<int>> GetUserCountByOrgIdAsync(long orgId)
        {
            try
            {
                // 这里需要注入用户仓储来查询用户数量
                // 暂时返回0
                return ApiResult<int>.Succeed(0);
            }
            catch (Exception ex)
            {
                return ApiResult<int>.Failed($"获取用户数量失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新组织状态
        /// </summary>
        public async Task<ApiResult> UpdateOrganizationStatusAsync(long id, bool status)
        {
            try
            {
                var organization = await _organizationRepository.GetByIdAsync(id);
                if (organization == null)
                {
                    return ApiResult.Failed("组织不存在");
                }

                organization.Status = status;
                await _organizationRepository.UpdateAsync(organization);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("状态更新成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"更新状态失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取所有组织（扁平列表）
        /// </summary>
        public async Task<ApiResult<List<OrganizationDto>>> GetAllOrganizationsAsync()
        {
            try
            {
                var organizations = await _organizationRepository.GetAllAsync();
                var dtos = organizations.Adapt<List<OrganizationDto>>();
                
                return ApiResult<List<OrganizationDto>>.Succeed(dtos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<OrganizationDto>>.Failed($"获取组织列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 移动组织到新的父组织下
        /// </summary>
        public async Task<ApiResult> MoveOrganizationAsync(long orgId, long? newParentId)
        {
            try
            {
                var organization = await _organizationRepository.GetByIdAsync(orgId);
                if (organization == null)
                {
                    return ApiResult.Failed("组织不存在");
                }

                if (newParentId.HasValue)
                {
                    var newParent = await _organizationRepository.GetByIdAsync(newParentId.Value);
                    if (newParent == null)
                    {
                        return ApiResult.Failed("新父组织不存在");
                    }

                    if (newParentId.Value == orgId)
                    {
                        return ApiResult.Failed("不能将组织移动到自己下面");
                    }
                }

                organization.ParentId = newParentId;
                await SetOrganizationHierarchyAsync(organization);
                
                await _organizationRepository.UpdateAsync(organization);
                await _unitOfWork.SaveChangesAsync();

                return ApiResult.Succeed("组织移动成功");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"移动组织失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查是否可以删除组织（是否有子组织或用户）
        /// </summary>
        public async Task<ApiResult<bool>> CanDeleteOrganizationAsync(long orgId)
        {
            try
            {
                // 检查是否有子组织
                var children = await _organizationRepository.GetChildOrganizationsAsync(orgId);
                if (children.Any())
                {
                    return ApiResult<bool>.Succeed(false);
                }

                // 这里需要检查是否有用户，暂时返回true
                return ApiResult<bool>.Succeed(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Failed($"检查删除权限失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 分页查询组织
        /// </summary>
        public async Task<ApiResult<PagedResult<OrganizationDto>>> GetPagedOrganizationsAsync(OrganizationQueryDto query)
        {
            try
            {
                // 这里需要实现分页查询逻辑
                // 暂时返回空结果
                var pagedResult = PagedResult<OrganizationDto>.Empty(query.PageIndex, query.PageSize);
                return ApiResult<PagedResult<OrganizationDto>>.Succeed(pagedResult);
            }
            catch (Exception ex)
            {
                return ApiResult<PagedResult<OrganizationDto>>.Failed($"查询组织失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 设置组织路径信息
        /// </summary>
        private async Task SetOrganizationHierarchyAsync(Organization organization)
        {
            if (organization.ParentId.HasValue)
            {
                var parent = await _organizationRepository.GetByIdAsync(organization.ParentId.Value);
                if (parent != null)
                {
                    organization.ParentIds = string.IsNullOrWhiteSpace(parent.ParentIds)
                        ? parent.Id.ToString()
                        : $"{parent.ParentIds},{parent.Id}";
                }
            }
            else
            {
                organization.ParentIds = "0";
            }
        }
    }
}
