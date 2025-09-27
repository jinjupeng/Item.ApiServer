using ApiServer.Application.DTOs.Common;
using ApiServer.Domain.Enums;

namespace ApiServer.Application.DTOs.Menu
{
    /// <summary>
    /// 菜单DTO
    /// </summary>
    public class MenuDto : AuditableDto
    {
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string? MenuCode { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; } = string.Empty;

        /// <summary>
        /// 父菜单ID
        /// </summary>
        public long? MenuPid { get; set; }

        /// <summary>
        /// 父菜单名称
        /// </summary>
        public string? ParentMenuName { get; set; }

        /// <summary>
        /// 所有父节点ID
        /// </summary>
        public string MenuPids { get; set; } = string.Empty;

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
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 跳转URL
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<MenuDto> Children { get; set; } = new();
    }

    /// <summary>
    /// 菜单树节点DTO
    /// </summary>
    public class MenuTreeDto
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; } = string.Empty;

        /// <summary>
        /// 菜单编码
        /// </summary>
        public string? MenuCode { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 跳转URL
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool Expanded { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<MenuTreeDto> Children { get; set; } = new();
    }

    /// <summary>
    /// 创建菜单DTO
    /// </summary>
    public class CreateMenuDto
    {
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string? MenuCode { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; } = string.Empty;

        /// <summary>
        /// 父菜单ID
        /// </summary>
        public long? MenuPid { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 跳转URL
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }
    }

    /// <summary>
    /// 更新菜单DTO
    /// </summary>
    public class UpdateMenuDto
    {
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string? MenuCode { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; } = string.Empty;

        /// <summary>
        /// 父菜单ID
        /// </summary>
        public long? MenuPid { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 跳转URL
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType MenuType { get; set; }
    }

    /// <summary>
    /// 菜单查询DTO
    /// </summary>
    public class MenuQueryDto
    {
        /// <summary>
        /// 菜单名称（模糊查询）
        /// </summary>
        public string? MenuName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public MenuType? MenuType { get; set; }
    }
}
