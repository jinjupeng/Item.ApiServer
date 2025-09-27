using ApiServer.Application.DTOs.Organization;
using ApiServer.Shared.Common;

namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 组织服务接口
    /// </summary>
    public interface IOrganizationService
    {
        /// <summary>
        /// 创建组织
        /// </summary>
        Task<ApiResult<long>> CreateOrganizationAsync(CreateOrganizationDto dto);

        /// <summary>
        /// 更新组织
        /// </summary>
        Task<ApiResult> UpdateOrganizationAsync(long id, UpdateOrganizationDto dto);

        /// <summary>
        /// 删除组织
        /// </summary>
        Task<ApiResult> DeleteOrganizationAsync(long id);

        /// <summary>
        /// 获取组织详情
        /// </summary>
        Task<ApiResult<OrganizationDto>> GetOrganizationByIdAsync(long id);

        /// <summary>
        /// 获取组织树
        /// </summary>
        Task<ApiResult<List<OrganizationTreeDto>>> GetOrganizationTreeAsync(OrganizationQueryDto? query = null);

        /// <summary>
        /// 根据父ID获取子组织
        /// </summary>
        Task<ApiResult<List<OrganizationDto>>> GetChildOrganizationsAsync(long parentId);

        /// <summary>
        /// 检查组织编码是否存在
        /// </summary>
        Task<ApiResult<bool>> OrgCodeExistsAsync(string orgCode, long? excludeId = null);

        /// <summary>
        /// 获取父组织列表
        /// </summary>
        Task<ApiResult<List<OrganizationDto>>> GetParentOrganizationsAsync();

        /// <summary>
        /// 更新组织排序
        /// </summary>
        Task<ApiResult> UpdateOrganizationSortAsync(long orgId, int sort);

        /// <summary>
        /// 获取组织下的用户数量
        /// </summary>
        Task<ApiResult<int>> GetUserCountByOrgIdAsync(long orgId);

        /// <summary>
        /// 更新组织状态
        /// </summary>
        Task<ApiResult> UpdateOrganizationStatusAsync(long id, bool status);

        /// <summary>
        /// 获取所有组织（扁平列表）
        /// </summary>
        Task<ApiResult<List<OrganizationDto>>> GetAllOrganizationsAsync();

        /// <summary>
        /// 移动组织到新的父组织下
        /// </summary>
        Task<ApiResult> MoveOrganizationAsync(long orgId, long? newParentId);

        /// <summary>
        /// 检查是否可以删除组织（是否有子组织或用户）
        /// </summary>
        Task<ApiResult<bool>> CanDeleteOrganizationAsync(long orgId);
    }
}
