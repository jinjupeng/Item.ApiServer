using ApiServer.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiServer.WebApi.Authorization
{
    /// <summary>
    /// 权限验证处理器：从当前用户的声明或其他来源判断是否拥有权限
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ICurrentUser _currentUser;

        public PermissionAuthorizationHandler(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // 简化：从用户声明中读取自定义权限列表声明("permissions")，逗号分隔
            var permissions = _currentUser.GetClaim("permissions");
            if (!string.IsNullOrWhiteSpace(permissions))
            {
                var list = permissions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (list.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            // 可选：超级管理员角色放行
            if (_currentUser.HasRole("super_admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
