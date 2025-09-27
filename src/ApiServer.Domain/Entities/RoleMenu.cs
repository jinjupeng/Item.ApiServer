using ApiServer.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer.Domain.Entities
{
    /// <summary>
    /// 角色菜单关联实体
    /// </summary>
    public class RoleMenu : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        public long MenuId { get; set; }

        // 导航属性
        /// <summary>
        /// 角色
        /// </summary>
        public virtual Role Role { get; set; } = null!;

        /// <summary>
        /// 菜单
        /// </summary>
        public virtual Menu Menu { get; set; } = null!;
    }
}
