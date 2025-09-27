using System.ComponentModel.DataAnnotations;

namespace ApiServer.Application.DTOs.User
{
    /// <summary>
    /// 更新用户DTO
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(50, ErrorMessage = "用户名长度不能超过50个字符")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码（可选，如果不提供则不更新密码）
        /// </summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
        public string? Password { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(50, ErrorMessage = "昵称长度不能超过50个字符")]
        public string Nickname { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        [StringLength(100, ErrorMessage = "邮箱长度不能超过100个字符")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        [Phone(ErrorMessage = "手机号格式不正确")]
        [StringLength(20, ErrorMessage = "手机号长度不能超过20个字符")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// 头像URL
        /// </summary>
        [StringLength(200, ErrorMessage = "头像URL长度不能超过200个字符")]
        public string Avatar { get; set; } = string.Empty;

        /// <summary>
        /// 组织ID
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<long>? RoleIds { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
        public string Remark { get; set; } = string.Empty;
    }
}