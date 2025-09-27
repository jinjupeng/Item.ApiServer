using ApiServer.Domain.Common;
using ApiServer.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Domain.Entities
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class User : SoftDeleteEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(20)]
        public string? NickName { get; set; }
        
        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(512)]
        public string? Portrait { get; set; }
        
        /// <summary>
        /// 组织ID
        /// </summary>
        public long OrgId { get; set; }
        
        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Enabled;
        
        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(13)]
        public string? Phone { get; set; }
        
        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(64)]
        [EmailAddress]
        public string? Email { get; set; }

        // 导航属性
        /// <summary>
        /// 所属组织
        /// </summary>
        public virtual Organization? Organization { get; set; }

        /// <summary>
        /// 用户角色关联
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}