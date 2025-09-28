using Microsoft.AspNetCore.Authorization;

namespace ApiServer.WebApi.Authorization
{
    /// <summary>
    /// �����ڶ���������������������Ȩ����
    /// </summary>
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private const string PolicyPrefix = "PERMISSION:";

        public string Permission { get; }

        public PermissionAuthorizeAttribute(string permission)
        {
            Permission = permission;
            Policy = PolicyPrefix + permission;
        }

        public static bool TryResolvePolicy(string? policy, out string? permission)
        {
            permission = null;
            if (string.IsNullOrWhiteSpace(policy)) return false;
            if (!policy.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase)) return false;
            permission = policy.Substring(PolicyPrefix.Length);
            return true;
        }
    }
}
