using ApiServer.Application.DTOs.Common;

namespace ApiServer.Application.DTOs.Role
{
    /// <summary>
    /// 角色DTO
    /// </summary>
    public class RoleDto : AuditableDto
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 角色描述
        /// </summary>
        public string? RoleDesc { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// 用户数量
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 菜单权限ID列表
        /// </summary>
        public List<long> MenuIds { get; set; } = new();

        /// <summary>
        /// API权限ID列表
        /// </summary>
        public List<long> ApiIds { get; set; } = new();
    }

    /// <summary>
    /// 创建角色DTO
    /// </summary>
    public class CreateRoleDto
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 角色描述
        /// </summary>
        public string? RoleDesc { get; set; }

        /// <summary>
        /// 菜单权限ID列表
        /// </summary>
        public List<long> MenuIds { get; set; } = new();

        /// <summary>
        /// API权限ID列表
        /// </summary>
        public List<long> ApiIds { get; set; } = new();
    }

    /// <summary>
    /// 更新角色DTO
    /// </summary>
    public class UpdateRoleDto
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 角色描述
        /// </summary>
        public string? RoleDesc { get; set; }

        /// <summary>
        /// 菜单权限ID列表
        /// </summary>
        public List<long> MenuIds { get; set; } = new();

        /// <summary>
        /// API权限ID列表
        /// </summary>
        public List<long> ApiIds { get; set; } = new();
    }

    /// <summary>
    /// 角色查询DTO
    /// </summary>
    public class RoleQueryDto : PagedQueryDto
    {
        /// <summary>
        /// 角色名称（模糊查询）
        /// </summary>
        public string? RoleName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? Status { get; set; }
    }

    /// <summary>
    /// 用户角色分配DTO
    /// </summary>
    public class UserRoleAssignDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<long> RoleIds { get; set; } = new();
    }

    /// <summary>
    /// 角色权限DTO
    /// </summary>
    public class RolePermissionDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 菜单权限ID列表
        /// </summary>
        public List<long> MenuIds { get; set; } = new();

        /// <summary>
        /// API权限ID列表
        /// </summary>
        public List<long> ApiIds { get; set; } = new();
    }
}
