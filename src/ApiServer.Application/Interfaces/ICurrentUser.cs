using System.Security.Claims;

namespace ApiServer.Application.Interfaces
{
    /// <summary>
    /// 当前用户上下文接口
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// 是否已认证
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// 用户ID（来自令牌声明）
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// 用户名（来自令牌声明）
        /// </summary>
        string? Username { get; }

        /// <summary>
        /// 角色列表
        /// </summary>
        IEnumerable<string> Roles { get; }

        /// <summary>
        /// 原始访问令牌（Bearer Token）
        /// </summary>
        string? Token { get; }

        /// <summary>
        /// 全部声明
        /// </summary>
        IEnumerable<Claim> Claims { get; }

        /// <summary>
        /// 获取指定类型的声明值
        /// </summary>
        string? GetClaim(string type);

        /// <summary>
        /// 是否拥有指定角色
        /// </summary>
        bool HasRole(string role);
    }
}
