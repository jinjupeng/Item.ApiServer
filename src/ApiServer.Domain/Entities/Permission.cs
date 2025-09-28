using ApiServer.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Domain.Entities
{
    /// <summary>
    /// 接口权限
    /// </summary>
    public class Permission : SoftDeleteEntity
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 编码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 父接口ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 所有父节点ID
        /// </summary>
        [Required]
        [StringLength(128)]
        public string ParentIds { get; set; } = string.Empty;

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 请求地址，如 /api/users
        /// </summary>
        [StringLength(64)]
        public string? Url { get; set; }

        // 导航属性
        /// <summary>
        /// 父API
        /// </summary>
        public virtual Permission? Parent { get; set; }

        /// <summary>
        /// 子API列表
        /// </summary>
        public virtual ICollection<Permission> Children { get; set; } = new List<Permission>();

        /// <summary>
        /// 角色API关联
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
