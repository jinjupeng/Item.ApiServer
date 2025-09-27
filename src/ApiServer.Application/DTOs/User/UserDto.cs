using ApiServer.Domain.Enums;

namespace ApiServer.Application.DTOs.User
{
    /// <summary>
    /// 用户DTO
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 头像URL
        /// </summary>
        public string Avatar { get; set; } = string.Empty;

        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string OrgName { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<long> RoleIds { get; set; } = new();

        /// <summary>
        /// 角色名称列表
        /// </summary>
        public List<string> RoleNames { get; set; } = new();

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LastLoginIp { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModifiedTime { get; set; }
    }
}