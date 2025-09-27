using ApiServer.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Domain.Entities
{
    /// <summary>
    /// API接口实体
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
        /// 是否叶子节点
        /// </summary>
        public bool IsLeaf { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 跳转URL
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
        public virtual ICollection<RolePermission> RoleApis { get; set; } = new List<RolePermission>();
    }
}
