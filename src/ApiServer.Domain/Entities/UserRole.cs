using ApiServer.Domain.Common;

namespace ApiServer.Domain.Entities
{
    /// <summary>
    /// 用户角色关联实体
    /// </summary>
    public class UserRole : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        // 导航属性
        /// <summary>
        /// 用户
        /// </summary>
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// 角色
        /// </summary>
        public virtual Role Role { get; set; } = null!;
    }

}
