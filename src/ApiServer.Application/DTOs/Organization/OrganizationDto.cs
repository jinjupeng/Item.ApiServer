using ApiServer.Application.DTOs.Common;

namespace ApiServer.Application.DTOs.Organization
{
    /// <summary>
    /// 组织DTO
    /// </summary>
    public class OrganizationDto : AuditableDto
    {
        /// <summary>
        /// 组织编码
        /// </summary>
        public string? OrgCode { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string? OrgName { get; set; }

        /// <summary>
        /// 父组织ID
        /// </summary>
        public long? OrgPid { get; set; }

        /// <summary>
        /// 父组织名称
        /// </summary>
        public string? ParentOrgName { get; set; }

        /// <summary>
        /// 所有父节点ID
        /// </summary>
        public string OrgPids { get; set; } = string.Empty;

        /// <summary>
        /// 是否叶子节点
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 电话
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 用户数量
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 子组织列表
        /// </summary>
        public List<OrganizationDto> Children { get; set; } = new();
    }

    /// <summary>
    /// 组织树节点DTO
    /// </summary>
    public class OrganizationTreeDto
    {
        /// <summary>
        /// 组织ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 组织编码
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// 父组织ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 用户数量
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool Expanded { get; set; }

        /// <summary>
        /// 子组织列表
        /// </summary>
        public List<OrganizationTreeDto> Children { get; set; } = new();
    }

    /// <summary>
    /// 创建组织DTO
    /// </summary>
    public class CreateOrganizationDto
    {
        /// <summary>
        /// 组织编码
        /// </summary>
        public string? OrgCode { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; } = string.Empty;

        /// <summary>
        /// 父组织ID
        /// </summary>
        public long? OrgPid { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }
    }

    /// <summary>
    /// 更新组织DTO
    /// </summary>
    public class UpdateOrganizationDto
    {
        /// <summary>
        /// 组织编码
        /// </summary>
        public string? OrgCode { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; } = string.Empty;

        /// <summary>
        /// 父组织ID
        /// </summary>
        public long? OrgPid { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }
    }

    /// <summary>
    /// 组织查询DTO
    /// </summary>
    public class OrganizationQueryDto
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 组织名称（模糊查询）
        /// </summary>
        public string? OrgName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// 父组织ID
        /// </summary>
        public long? ParentId { get; set; }
    }
}
