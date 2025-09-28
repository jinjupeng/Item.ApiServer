using System.ComponentModel.DataAnnotations;

namespace ApiServer.Application.DTOs.Users
{
    /// <summary>
    /// 用户DTO基类
    /// </summary>
    public abstract class BaseUserDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(20, ErrorMessage = "用户名长度不能超过20个字符")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(20, ErrorMessage = "昵称长度不能超过20个字符")]
        public string? NickName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Phone(ErrorMessage = "电话号码格式不正确")]
        [StringLength(13, ErrorMessage = "电话号码长度不能超过13个字符")]
        public string? Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        [StringLength(64, ErrorMessage = "邮箱长度不能超过64个字符")]
        public string? Email { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; } = true;
    }

    /// <summary>
    /// 创建用户DTO
    /// </summary>
    public class CreateUserDto : BaseUserDto
    {
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码长度必须在6-20个字符之间")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare(nameof(Password), ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<long> RoleIds { get; set; } = new();
    }

    /// <summary>
    /// 更新用户DTO
    /// </summary>
    public class UpdateUserDto : BaseUserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(512, ErrorMessage = "头像URL长度不能超过512个字符")]
        public string? Portrait { get; set; }
    }

    /// <summary>
    /// 用户查询DTO
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
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Portrait { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string? OrgName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<RoleDto> Roles { get; set; } = new();
    }

    /// <summary>
    /// 用户查询条件DTO
    /// </summary>
    public class UserQueryDto
    {
        /// <summary>
        /// 用户名（模糊查询）
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 昵称（模糊查询）
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// 修改密码DTO
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required(ErrorMessage = "旧密码不能为空")]
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "新密码不能为空")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码长度必须在6-20个字符之间")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// 确认新密码
        /// </summary>
        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare(nameof(NewPassword), ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// 重置密码DTO
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "新密码不能为空")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码长度必须在6-20个字符之间")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// 确认新密码
        /// </summary>
        [Required(ErrorMessage = "确认密码不能为空")]
        [Compare(nameof(NewPassword), ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// 角色DTO（简化版）
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 角色描述
        /// </summary>
        public string? RoleDesc { get; set; }
    }
}