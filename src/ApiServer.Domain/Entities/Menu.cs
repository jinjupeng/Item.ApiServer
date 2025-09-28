using ApiServer.Domain.Common;
using ApiServer.Domain.Enums;
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

        /**
         
        Directory（目录/分组）
        // 示例：系统管理
        {
          name: '系统管理',
          type: MenuType.Directory,
          icon: 'Setting',
          children: [...]  // 包含子菜单
        }
        作用：纯粹的分组容器，用于组织和分类
        特点：不对应具体页面，只用于展示层级结构
        渲染：通常显示为可展开的文件夹图标

        Menu（菜单/页面）
        // 示例：用户管理页面
        {
          name: '用户管理',
          type: MenuType.Menu,
          path: '/system/users',
          component: 'UserManagement'
        }

        作用：对应具体的页面路由
        特点：点击后跳转到对应页面
        渲染：显示为可点击的导航项

        Button（按钮/权限点）

        // 示例：删除用户按钮
        {
          name: '删除用户',
          type: MenuType.Button,
          permission: 'user:delete',
          parent: 'user-management'
        }

        作用：页面内的操作权限控制
        特点：不显示在侧边栏，用于控制按钮显示/隐藏
        渲染：不在菜单中显示，但影响页面内按钮权限
         **/
        /// <summary>
        /// 菜单类型，目录、菜单、按钮
        /// </summary>
        public MenuType Type { get; set; } = MenuType.Menu;

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