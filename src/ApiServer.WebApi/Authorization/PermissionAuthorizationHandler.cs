using ApiServer.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ApiServer.WebApi.Authorization
{
    /// <summary>
    /// Ȩ����֤���������ӵ�ǰ�û���������������Դ�ж��Ƿ�ӵ��Ȩ��
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
            // �򻯣����û������ж�ȡ�Զ���Ȩ���б�����("permissions")�����ŷָ�
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

            // ��ѡ����������Ա��ɫ����
            if (_currentUser.HasRole("super_admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
