using ApiServer.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Domain.Entities
{
    /// <summary>
    /// 角色实体
    /// </summary>
    public class Role : SoftDeleteEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [StringLength(32)]
        public string RoleName { get; set; } = string.Empty;
        
        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(128)]
        public string? RoleDesc { get; set; }
        
        /// <summary>
        /// 状态（是否启用）
        /// </summary>
        public bool Status { get; set; } = true;

        // 导航属性
        /// <summary>
        /// 用户角色关联
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// 角色菜单关联
        /// </summary>
        public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

        /// <summary>
        /// 角色API关联
        /// </summary>
        public virtual ICollection<RolePermission> RoleApis { get; set; } = new List<RolePermission>();
    }
}