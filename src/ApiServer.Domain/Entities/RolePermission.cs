using ApiServer.Domain.Common;

namespace ApiServer.Domain.Entities
{

    /// <summary>
    /// 角色和接口api关联实体
    /// </summary>
    public class RolePermission : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// Permission ID
        /// </summary>
        public long PermissionId { get; set; }

        // 导航属性
        /// <summary>
        /// 角色
        /// </summary>
        public virtual Role Role { get; set; } = null!;

        /// <summary>
        /// API
        /// </summary>
        public virtual Permission Permission { get; set; } = null!;
    }
}
