using ApiServer.Domain.Entities;

namespace ApiServer.Application.Interfaces.Repositories
{
    /// <summary>
    /// 组织仓储接口
    /// </summary>
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
        /// <summary>
        /// 根据组织编码获取组织
        /// </summary>
        Task<Organization?> GetByOrgCodeAsync(string orgCode);

        /// <summary>
        /// 检查组织编码是否存在
        /// </summary>
        Task<bool> IsOrgCodeExistsAsync(string orgCode, long? excludeOrgId = null);

        /// <summary>
        /// 获取组织树
        /// </summary>
        Task<IEnumerable<Organization>> GetOrganizationTreeAsync(string? orgName = null, bool? status = null);

        /// <summary>
        /// 根据父ID获取子组织
        /// </summary>
        Task<IEnumerable<Organization>> GetChildOrganizationsAsync(long parentId);

        /// <summary>
        /// 获取父组织列表
        /// </summary>
        Task<IEnumerable<Organization>> GetParentOrganizationsAsync();

        /// <summary>
        /// 更新组织排序
        /// </summary>
        Task UpdateOrganizationSortAsync(long orgId, int sort);

        /// <summary>
        /// 获取组织下的用户数量
        /// </summary>
        Task<int> GetUserCountByOrgIdAsync(long orgId);

        /// <summary>
        /// 获取最大排序值
        /// </summary>
        Task<int> GetMaxSortAsync(long? parentId = null);

        /// <summary>
        /// 检查是否有子组织
        /// </summary>
        Task<bool> HasChildOrganizationsAsync(long orgId);

        /// <summary>
        /// 获取所有父组织（非叶子节点）
        /// </summary>
        Task<IEnumerable<Organization>> GetAllParentOrganizationsAsync();

        /// <summary>
        /// 批量更新组织状态
        /// </summary>
        Task BatchUpdateStatusAsync(IEnumerable<long> orgIds, bool status);

        /// <summary>
        /// 获取组织的所有子组织ID（包括子子组织）
        /// </summary>
        Task<IEnumerable<long>> GetAllChildOrgIdsAsync(long orgId);

        /// <summary>
        /// 移动组织到新的父组织下
        /// </summary>
        Task MoveOrganizationAsync(long orgId, long? newParentId);

        /// <summary>
        /// 更新组织路径信息
        /// </summary>
        Task UpdateOrganizationPathAsync(long orgId, string orgPids);
    }
}
