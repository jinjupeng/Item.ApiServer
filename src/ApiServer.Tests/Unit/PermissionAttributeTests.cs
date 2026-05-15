using System.Reflection;
using ApiServer.WebApi.Authorization;
using ApiServer.WebApi.Controllers;
using FluentAssertions;
using Xunit;

namespace ApiServer.Tests.Unit;

public class PermissionAttributeTests
{
    public static IEnumerable<object[]> ProtectedActions()
    {
        yield return new object[] { typeof(UsersController), nameof(UsersController.GetUsers), "system:user:list" };
        yield return new object[] { typeof(UsersController), nameof(UsersController.GetUser), "system:user:list" };
        yield return new object[] { typeof(UsersController), nameof(UsersController.CreateUser), "system:user:create" };
        yield return new object[] { typeof(UsersController), nameof(UsersController.UpdateUser), "system:user:update" };
        yield return new object[] { typeof(UsersController), nameof(UsersController.DeleteUser), "system:user:delete" };
        yield return new object[] { typeof(UsersController), nameof(UsersController.UpdateUserStatus), "system:user:update" };
        yield return new object[] { typeof(UsersController), nameof(UsersController.ResetPassword), "system:user:update" };

        yield return new object[] { typeof(RolesController), nameof(RolesController.GetRoles), "system:role:list" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.GetRole), "system:role:list" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.CreateRole), "system:role:create" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.UpdateRole), "system:role:update" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.DeleteRole), "system:role:delete" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.UpdateRoleStatus), "system:role:update" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.AssignRolesToUser), "system:role:update" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.RemoveRolesFromUser), "system:role:update" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.GetRolePermissions), "system:role:list" };
        yield return new object[] { typeof(RolesController), nameof(RolesController.AssignPermissionsToRole), "system:role:update" };

        yield return new object[] { typeof(MenusController), nameof(MenusController.GetMenuTree), "system:menu:list" };
        yield return new object[] { typeof(MenusController), nameof(MenusController.GetMenu), "system:menu:list" };
        yield return new object[] { typeof(MenusController), nameof(MenusController.CreateMenu), "system:menu:create" };
        yield return new object[] { typeof(MenusController), nameof(MenusController.UpdateMenu), "system:menu:update" };
        yield return new object[] { typeof(MenusController), nameof(MenusController.DeleteMenu), "system:menu:delete" };
        yield return new object[] { typeof(MenusController), nameof(MenusController.UpdateMenuStatus), "system:menu:update" };
        yield return new object[] { typeof(MenusController), nameof(MenusController.UpdateMenuSort), "system:menu:update" };

        yield return new object[] { typeof(AuditLogsController), nameof(AuditLogsController.GetAuditLogs), "system:auditlog:list" };
        yield return new object[] { typeof(AuditLogsController), nameof(AuditLogsController.GetAuditLogById), "system:auditlog:list" };
        yield return new object[] { typeof(AuditLogsController), nameof(AuditLogsController.ExportAuditLogs), "system:auditlog:create" };
        yield return new object[] { typeof(AuditLogsController), nameof(AuditLogsController.GetAuditLogStatistics), "system:auditlog:list" };
        yield return new object[] { typeof(AuditLogsController), nameof(AuditLogsController.CleanupExpiredLogs), "system:auditlog:update" };
    }

    [Theory]
    [MemberData(nameof(ProtectedActions))]
    public void Action_ShouldDeclareExpectedPermission(Type controllerType, string methodName, string permission)
    {
        var method = controllerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull($"{controllerType.Name}.{methodName} should exist");

        var attribute = method!.GetCustomAttribute<PermissionAuthorizeAttribute>();

        attribute.Should().NotBeNull($"{controllerType.Name}.{methodName} should be protected by a permission attribute");
        attribute!.Permission.Should().Be(permission);
    }
}
