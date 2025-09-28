using Microsoft.AspNetCore.Authorization;

namespace ApiServer.WebApi.Authorization
{
    /// <summary>
    /// 权限策略需求，携带权限码
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
