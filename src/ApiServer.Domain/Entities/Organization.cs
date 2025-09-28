using ApiServer.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Domain.Entities
{
    /// <summary>
    /// 组织实体
    /// </summary>
    public class Organization : SoftDeleteEntity
    {
        /// <summary>
        /// 组织编码
        /// </summary>
        [StringLength(32)]
        public string? Code { get; set; }
        
        /// <summary>
        /// 组织名称
        /// </summary>
        [Required]
        [StringLength(32)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 父组织ID
        /// </summary>
        public long? ParentId { get; set; }
        
        /// <summary>
        /// 所有父节点ID路径
        /// </summary>
        [Required]
        [StringLength(128)]
        public string ParentIds { get; set; } = string.Empty;
        
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; } = 0;
        
        /// <summary>
        /// 状态（是否启用）
        /// </summary>
        public bool Status { get; set; } = true;
        
        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(13)]
        public string? Phone { get; set; }
        
        /// <summary>
        /// 传真
        /// </summary>
        [StringLength(16)]
        public string? Fax { get; set; }
        
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(64)]
        public string? Address { get; set; }

        // 导航属性
        /// <summary>
        /// 父组织
        /// </summary>
        public virtual Organization? Parent { get; set; }

        /// <summary>
        /// 子组织
        /// </summary>
        public virtual ICollection<Organization> Children { get; set; } = new List<Organization>();

        /// <summary>
        /// 组织下的用户
        /// </summary>
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}