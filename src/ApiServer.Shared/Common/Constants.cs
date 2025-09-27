namespace ApiServer.Shared.Common
{
    /// <summary>
    /// 系统常量
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 默认页大小
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// 最大页大小
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// 默认密码
        /// </summary>
        public const string DefaultPassword = "123456";

        /// <summary>
        /// 超级管理员角色
        /// </summary>
        public const string SuperAdminRole = "SuperAdmin";

        /// <summary>
        /// 管理员角色
        /// </summary>
        public const string AdminRole = "Admin";

        /// <summary>
        /// 普通用户角色
        /// </summary>
        public const string UserRole = "User";

        /// <summary>
        /// JWT Claims
        /// </summary>
        public static class Claims
        {
            public const string UserId = "userId";
            public const string Username = "username";
            public const string OrgId = "orgId";
            public const string Roles = "roles";
            public const string Permissions = "permissions";
        }

        /// <summary>
        /// 缓存键
        /// </summary>
        public static class CacheKeys
        {
            public const string UserPrefix = "user:";
            public const string RolePrefix = "role:";
            public const string MenuPrefix = "menu:";
            public const string ConfigPrefix = "config:";
            public const string DictPrefix = "dict:";
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        public static class ErrorCodes
        {
            public const string ValidationFailed = "VALIDATION_FAILED";
            public const string NotFound = "NOT_FOUND";
            public const string Unauthorized = "UNAUTHORIZED";
            public const string Forbidden = "FORBIDDEN";
            public const string DuplicateResource = "DUPLICATE_RESOURCE";
            public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        }
    }
}