using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Domain.Entities;
using ApiServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Infrastructure.Repositories
{
    /// <summary>
    /// 组织仓储实现
    /// </summary>
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据父组织ID获取子组织列表
        /// </summary>
        public async Task<IEnumerable<Organization>> GetByParentIdAsync(long? parentId)
        {
            var query = _dbSet.AsQueryable();
            
            if (parentId.HasValue)
            {
                query = query.Where(o => o.ParentId == parentId.Value);
            }
            else
            {
                query = query.Where(o => o.ParentId == null);
            }

            return await query
                .Where(o => !o.IsDeleted && o.Status)
                .OrderBy(o => o.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 获取组织树
        /// </summary>
        public async Task<IEnumerable<Organization>> GetOrganizationTreeAsync()
        {
            var allOrgs = await _dbSet
                .Where(o => !o.IsDeleted && o.Status)
                .OrderBy(o => o.Sort)
                .ToListAsync();

            return BuildOrganizationTree(allOrgs, null);
        }

        /// <summary>
        /// 根据组织编码获取组织
        /// </summary>
        public async Task<Organization?> GetByCodeAsync(string orgCode)
        {
            return await _dbSet
                .Where(o => o.Code == orgCode && !o.IsDeleted)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据组织编码获取组织
        /// </summary>
        public async Task<Organization?> GetByOrgCodeAsync(string orgCode)
        {
            return await GetByCodeAsync(orgCode);
        }

        /// <summary>
        /// 获取组织树（带过滤条件）
        /// </summary>
        public async Task<IEnumerable<Organization>> GetOrganizationTreeAsync(string? orgName = null, bool? status = null)
        {
            var query = _dbSet.Where(o => !o.IsDeleted);
            
            if (!string.IsNullOrEmpty(orgName))
            {
                query = query.Where(o => o.Name.Contains(orgName));
            }
            
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var allOrgs = await query.OrderBy(o => o.Sort).ToListAsync();
            return BuildOrganizationTree(allOrgs, null);
        }

        /// <summary>
        /// 根据父ID获取子组织
        /// </summary>
        public async Task<IEnumerable<Organization>> GetChildOrganizationsAsync(long parentId)
        {
            return await GetByParentIdAsync(parentId);
        }

        /// <summary>
        /// 获取父组织列表
        /// </summary>
        public async Task<IEnumerable<Organization>> GetParentOrganizationsAsync()
        {
            return await GetByParentIdAsync(null);
        }

        /// <summary>
        /// 更新组织排序
        /// </summary>
        public async Task UpdateOrganizationSortAsync(long orgId, int sort)
        {
            var organization = await GetByIdAsync(orgId);
            if (organization != null)
            {
                organization.Sort = sort;
                await UpdateAsync(organization);
            }
        }

        /// <summary>
        /// 获取组织下的用户数量
        /// </summary>
        public async Task<int> GetUserCountByOrgIdAsync(long orgId)
        {
            // 这里需要访问用户表，暂时返回0
            return 0;
        }

        /// <summary>
        /// 获取最大排序值
        /// </summary>
        public async Task<int> GetMaxSortAsync(long? parentId = null)
        {
            var query = _dbSet.Where(o => !o.IsDeleted);
            
            if (parentId.HasValue)
            {
                query = query.Where(o => o.ParentId == parentId.Value);
            }
            else
            {
                query = query.Where(o => o.ParentId == null);
            }

            return await query.MaxAsync(o => (int?)o.Sort) ?? 0;
        }

        /// <summary>
        /// 检查是否有子组织
        /// </summary>
        public async Task<bool> HasChildOrganizationsAsync(long orgId)
        {
            return await _dbSet.AnyAsync(o => o.ParentId == orgId && !o.IsDeleted);
        }

        /// <summary>
        /// 获取所有父组织（非叶子节点）
        /// </summary>
        public async Task<IEnumerable<Organization>> GetAllParentOrganizationsAsync()
        {
            return await _dbSet
                .Where(o => !o.IsDeleted && !o.IsLeaf)
                .OrderBy(o => o.Sort)
                .ToListAsync();
        }

        /// <summary>
        /// 批量更新组织状态
        /// </summary>
        public async Task BatchUpdateStatusAsync(IEnumerable<long> orgIds, bool status)
        {
            var organizations = await _dbSet
                .Where(o => orgIds.Contains(o.Id))
                .ToListAsync();

            foreach (var org in organizations)
            {
                org.Status = status;
            }

            await UpdateRangeAsync(organizations);
        }

        /// <summary>
        /// 获取组织的所有子组织ID（包括子子组织）
        /// </summary>
        public async Task<IEnumerable<long>> GetAllChildOrgIdsAsync(long orgId)
        {
            var result = new List<long>();
            var children = await GetByParentIdAsync(orgId);
            
            foreach (var child in children)
            {
                result.Add(child.Id);
                var grandChildren = await GetAllChildOrgIdsAsync(child.Id);
                result.AddRange(grandChildren);
            }

            return result;
        }

        /// <summary>
        /// 移动组织到新的父组织下
        /// </summary>
        public async Task MoveOrganizationAsync(long orgId, long? newParentId)
        {
            var organization = await GetByIdAsync(orgId);
            if (organization != null)
            {
                organization.ParentId = newParentId;
                // 这里应该重新计算层级和路径
                await UpdateAsync(organization);
            }
        }

        /// <summary>
        /// 更新组织路径信息
        /// </summary>
        public async Task UpdateOrganizationPathAsync(long orgId, string orgPids, int level, bool isLeaf)
        {
            var organization = await GetByIdAsync(orgId);
            if (organization != null)
            {
                organization.ParentIds = orgPids;
                organization.Level = level;
                organization.IsLeaf = isLeaf;
                await UpdateAsync(organization);
            }
        }

        /// <summary>
        /// 检查组织编码是否存在
        /// </summary>
        public async Task<bool> IsOrgCodeExistsAsync(string orgCode, long? excludeId = null)
        {
            var query = _dbSet.Where(o => o.Code == orgCode && !o.IsDeleted);
            
            if (excludeId.HasValue)
            {
                query = query.Where(o => o.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// 获取组织的所有子组织（递归）
        /// </summary>
        public async Task<IEnumerable<Organization>> GetAllChildrenAsync(long parentId)
        {
            var result = new List<Organization>();
            var children = await GetByParentIdAsync(parentId);
            
            foreach (var child in children)
            {
                result.Add(child);
                var grandChildren = await GetAllChildrenAsync(child.Id);
                result.AddRange(grandChildren);
            }

            return result;
        }

        /// <summary>
        /// 获取组织路径
        /// </summary>
        public async Task<string> GetOrganizationPathAsync(long orgId)
        {
            var org = await GetByIdAsync(orgId);
            if (org == null) return string.Empty;

            var path = new List<string> { org.Name };
            var currentOrg = org;

            while (currentOrg.ParentId.HasValue)
            {
                var parent = await GetByIdAsync(currentOrg.ParentId.Value);
                if (parent == null) break;
                
                path.Insert(0, parent.Name);
                currentOrg = parent;
            }

            return string.Join(" > ", path);
        }

        /// <summary>
        /// 构建组织树
        /// </summary>
        private IEnumerable<Organization> BuildOrganizationTree(IEnumerable<Organization> allOrgs, long? parentId)
        {
            return allOrgs
                .Where(o => o.ParentId == parentId)
                .Select(o =>
                {
                    o.Children = BuildOrganizationTree(allOrgs, o.Id).ToList();
                    return o;
                });
        }
    }
}
