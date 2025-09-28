using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ApiServer.WebApi.Authorization
{
    /// <summary>
    /// ��̬Ȩ�޲����ṩ�ߣ������� PERMISSION: ��ͷ�Ĳ���������̬��������
    /// </summary>
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public const string PolicyPrefix = "PERMISSION:";

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var permission = policyName.Substring(PolicyPrefix.Length);
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return base.GetPolicyAsync(policyName);
        }
    }
}
