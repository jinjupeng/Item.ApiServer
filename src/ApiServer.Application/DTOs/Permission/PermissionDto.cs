using ApiServer.Application.DTOs.Common;
using ApiServer.Domain.Enums;

namespace ApiServer.Application.DTOs.Permission
{
    /// <summary>
    /// 权限DTO
    /// </summary>
    public class PermissionDto : AuditableDto
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 权限描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        public PermissionType Type { get; set; }

        /// <summary>
        /// 资源
        /// </summary>
        public string? Resource { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
