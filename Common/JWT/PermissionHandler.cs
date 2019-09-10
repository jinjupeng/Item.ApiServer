using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Common.JWT
{
    /// <summary>
    /// 权限授权处理器
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // 赋值用户权限，也可直接从数据库获取
            var userPermissions = requirement.Permissions;
            //从AuthorizationHandlerContext转成HttpContext，以便取出表头信息
            var filterContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext);
            var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext;
            // 请求Url
            var questUrl = httpContext.Request.Path.Value.ToUpperInvariant();
            // 是否经过验证
            var isAuthenticated = httpContext.User.Identity.IsAuthenticated;
            if (isAuthenticated)
            {
                if (userPermissions.GroupBy(g => g.Url).Any(w => w.Key.ToUpperInvariant() == questUrl))
                {
                    // 用户名
                    var userName = httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value;
                    if (userPermissions.Any(w => w.Role == userName && w.Url.ToUpperInvariant() == questUrl))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        // 无权限跳转到拒绝页面
                        httpContext.Response.Redirect(requirement.DeniedAction);
                    }
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
