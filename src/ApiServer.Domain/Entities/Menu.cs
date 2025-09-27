using ApiServer.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Domain.Entities
{
    /// <summary>
    /// 菜单实体
    /// </summary>
    public class Menu : SoftDeleteEntity
    {
        /// <summary>
        /// 菜单编码
        /// </summary>
        [StringLength(64)]
        public string? Code { get; set; }
        
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required]
        [StringLength(32)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 父菜单ID
        /// </summary>
        public long? ParentId { get; set; }
        
        /// <summary>
        /// 所有父节点ID路径
        /// </summary>
        [Required]
        [StringLength(128)]
        public string ParentIds { get; set; } = string.Empty;
        
        /// <summary>
        /// 是否叶子节点
        /// </summary>
        public bool IsLeaf { get; set; } = true;
        
        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; } = 1;
        
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; } = 0;
        
        /// <summary>
        /// 状态（是否启用）
        /// </summary>
        public bool Status { get; set; } = true;
        
        /// <summary>
        /// 图标
        /// </summary>
        [StringLength(32)]
        public string? Icon { get; set; }
        
        /// <summary>
        /// 跳转URL
        /// </summary>
        [StringLength(64)]
        public string? Url { get; set; }

        // 导航属性
        /// <summary>
        /// 父菜单
        /// </summary>
        public virtual Menu? Parent { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();

        /// <summary>
        /// 角色菜单关联
        /// </summary>
        public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }

}