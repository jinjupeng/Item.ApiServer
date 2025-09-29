using ApiServer.Domain.Entities;
using ApiServer.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiServer.Infrastructure.Data
{
    /// <summary>
    /// 数据种子服务
    /// </summary>
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(ApplicationDbContext context, ILogger<DataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 初始化种子数据
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("开始初始化种子数据...");

                // 确保数据库已创建
                await _context.Database.EnsureCreatedAsync();

                // 初始化组织数据
                await SeedOrganizationsAsync();

                // 初始化角色数据
                await SeedRolesAsync();

                // 初始化用户数据
                await SeedUsersAsync();

                // 初始化权限数据（包含目录/菜单/按钮权限）
                await SeedPermissionsAsync();

                // 初始化用户角色关联
                await SeedUserRolesAsync();

                // 初始化角色权限关联
                await SeedRolePermissionsAsync();

                await _context.SaveChangesAsync();
                _logger.LogInformation("种子数据初始化完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "初始化种子数据时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 初始化组织数据
        /// </summary>
        private async Task SeedOrganizationsAsync()
        {
            if (await _context.Organizations.AnyAsync())
            {
                _logger.LogInformation("组织数据已存在，跳过初始化");
                return;
            }

            var organizations = new List<Organization>
            {
                new Organization
                {
                    Id = 1,
                    Code = "ROOT",
                    Name = "总公司",
                    ParentId = null,
                    ParentIds = "0",
                    Sort = 1,
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Organization
                {
                    Id = 2,
                    Code = "TECH",
                    Name = "技术部",
                    ParentId = 1,
                    ParentIds = "0,1",
                    Sort = 1,
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Organization
                {
                    Id = 3,
                    Code = "SALES",
                    Name = "销售部",
                    ParentId = 1,
                    ParentIds = "0,1",
                    Sort = 2,
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Organization
                {
                    Id = 4,
                    Code = "HR",
                    Name = "人事部",
                    ParentId = 1,
                    ParentIds = "0,1",
                    Sort = 3,
                    Status = true,
                    CreateTime = DateTime.Now
                }
            };

            await _context.Organizations.AddRangeAsync(organizations);
            _logger.LogInformation("已添加 {Count} 个组织", organizations.Count);
        }

        /// <summary>
        /// 初始化角色数据
        /// </summary>
        private async Task SeedRolesAsync()
        {
            if (await _context.Roles.AnyAsync())
            {
                _logger.LogInformation("角色数据已存在，跳过初始化");
                return;
            }

            var roles = new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = "超级管理员",
                    Code = "SUPER_ADMIN",
                    Desc = "系统超级管理员，拥有所有权限",
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Role
                {
                    Id = 2,
                    Name = "管理员",
                    Code = "ADMIN",
                    Desc = "系统管理员，拥有大部分权限",
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Role
                {
                    Id = 3,
                    Name = "普通用户",
                    Code = "USER",
                    Desc = "普通用户，拥有基本权限",
                    Status = true,
                    CreateTime = DateTime.Now
                }
            };

            await _context.Roles.AddRangeAsync(roles);
            _logger.LogInformation("已添加 {Count} 个角色", roles.Count);
        }

        /// <summary>
        /// 初始化用户数据
        /// </summary>
        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("用户数据已存在，跳过初始化");
                return;
            }

            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    NickName = "超级管理员",
                    OrgId = 1,
                    Status = UserStatus.Enabled,
                    Email = "admin@example.com",
                    Phone = "13800138000",
                    CreateTime = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Name = "manager",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    NickName = "部门经理",
                    OrgId = 2,
                    Status = UserStatus.Enabled,
                    Email = "manager@example.com",
                    Phone = "13800138001",
                    CreateTime = DateTime.Now
                },
                new User
                {
                    Id = 3,
                    Name = "user",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                    NickName = "普通用户",
                    OrgId = 2,
                    Status = UserStatus.Enabled,
                    Email = "user@example.com",
                    Phone = "13800138002",
                    CreateTime = DateTime.Now
                }
            };

            await _context.Users.AddRangeAsync(users);
            _logger.LogInformation("已添加 {Count} 个用户", users.Count);
        }

        /// <summary>
        /// 初始化权限数据（包含按钮权限点）
        /// </summary>
        private async Task SeedPermissionsAsync()
        {
            if (await _context.Permissions.AnyAsync())
            {
                _logger.LogInformation("权限数据已存在，跳过初始化");
                return;
            }

            var now = DateTime.Now;
            var permissions = new List<Permission>
            {
                // 根目录：系统管理
                new Permission { Id = 1, Code = "SYSTEM", Name = "系统管理", Type = PermissionType.Directory, Sort = 1, Status = true, Icon = "system", Url = "/system", CreateTime = now },

                // 一级菜单
                new Permission { Id = 2, Code = "USER_MANAGE", Name = "用户管理", Type = PermissionType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 1, Status = true, Icon = "user", Url = "/system/users", CreateTime = now },
                new Permission { Id = 3, Code = "ROLE_MANAGE", Name = "角色管理", Type = PermissionType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 2, Status = true, Icon = "role", Url = "/system/roles", CreateTime = now },
                new Permission { Id = 4, Code = "ORG_MANAGE",  Name = "组织管理", Type = PermissionType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 3, Status = true, Icon = "org",  Url = "/system/orgs",   CreateTime = now },
                new Permission { Id = 5, Code = "MENU_MANAGE", Name = "菜单管理", Type = PermissionType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 4, Status = true, Icon = "menu", Url = "/system/menus", CreateTime = now },
                new Permission { Id = 100, Code = "AUDIT_LOG", Name = "审计日志", Type = PermissionType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 4, Status = true, Icon = "log", Url = "/system/auditlogs", CreateTime = now },
                

                // 用户管理-按钮
                new Permission { Id = 6,  Code = "system:user:list",   Name = "用户查询", Type = PermissionType.Button, ParentId = 2, ParentIds = "0,1,2", Sort = 1, Status = true, CreateTime = now },
                new Permission { Id = 7,  Code = "system:user:create", Name = "用户新增", Type = PermissionType.Button, ParentId = 2, ParentIds = "0,1,2", Sort = 2, Status = true, CreateTime = now },
                new Permission { Id = 8,  Code = "system:user:update", Name = "用户修改", Type = PermissionType.Button, ParentId = 2, ParentIds = "0,1,2", Sort = 3, Status = true, CreateTime = now },
                new Permission { Id = 9,  Code = "system:user:delete", Name = "用户删除", Type = PermissionType.Button, ParentId = 2, ParentIds = "0,1,2", Sort = 4, Status = true, CreateTime = now },

                // 角色管理-按钮
                new Permission { Id = 10, Code = "system:role:list",   Name = "角色查询", Type = PermissionType.Button, ParentId = 3, ParentIds = "0,1,3", Sort = 1, Status = true, CreateTime = now },
                new Permission { Id = 11, Code = "system:role:create", Name = "角色新增", Type = PermissionType.Button, ParentId = 3, ParentIds = "0,1,3", Sort = 2, Status = true, CreateTime = now },
                new Permission { Id = 12, Code = "system:role:update", Name = "角色修改", Type = PermissionType.Button, ParentId = 3, ParentIds = "0,1,3", Sort = 3, Status = true, CreateTime = now },
                new Permission { Id = 13, Code = "system:role:delete", Name = "角色删除", Type = PermissionType.Button, ParentId = 3, ParentIds = "0,1,3", Sort = 4, Status = true, CreateTime = now },

                // 菜单管理-按钮
                new Permission { Id = 14, Code = "system:menu:list",   Name = "菜单查询", Type = PermissionType.Button, ParentId = 5, ParentIds = "0,1,5", Sort = 1, Status = true, CreateTime = now },
                new Permission { Id = 15, Code = "system:menu:create", Name = "菜单新增", Type = PermissionType.Button, ParentId = 5, ParentIds = "0,1,5", Sort = 2, Status = true, CreateTime = now },
                new Permission { Id = 16, Code = "system:menu:update", Name = "菜单修改", Type = PermissionType.Button, ParentId = 5, ParentIds = "0,1,5", Sort = 3, Status = true, CreateTime = now },
                new Permission { Id = 17, Code = "system:menu:delete", Name = "菜单删除", Type = PermissionType.Button, ParentId = 5, ParentIds = "0,1,5", Sort = 4, Status = true, CreateTime = now },

                // 组织管理-按钮
                new Permission { Id = 18, Code = "system:org:list",    Name = "组织查询", Type = PermissionType.Button, ParentId = 4, ParentIds = "0,1,4", Sort = 1, Status = true, CreateTime = now },
                new Permission { Id = 19, Code = "system:org:create",  Name = "组织新增", Type = PermissionType.Button, ParentId = 4, ParentIds = "0,1,4", Sort = 2, Status = true, CreateTime = now },
                new Permission { Id = 20, Code = "system:org:update",  Name = "组织修改", Type = PermissionType.Button, ParentId = 4, ParentIds = "0,1,4", Sort = 3, Status = true, CreateTime = now },
                new Permission { Id = 21, Code = "system:org:delete",  Name = "组织删除", Type = PermissionType.Button, ParentId = 4, ParentIds = "0,1,4", Sort = 4, Status = true, CreateTime = now },
            
                // 设计日志-按钮
                new Permission { Id = 22, Code = "system:auditlog:list",    Name = "日志查询", Type = PermissionType.Button, ParentId = 100, ParentIds = "0,1,100", Sort = 1, Status = true, CreateTime = now },
                new Permission { Id = 23, Code = "system:auditlog:create",  Name = "日志导出", Type = PermissionType.Button, ParentId = 100, ParentIds = "0,1,100", Sort = 2, Status = true, CreateTime = now },
                new Permission { Id = 24, Code = "system:auditlog:update",  Name = "日志清空", Type = PermissionType.Button, ParentId = 100, ParentIds = "0,1,100", Sort = 3, Status = true, CreateTime = now },
                new Permission { Id = 25, Code = "system:auditlog:delete",  Name = "日志删除", Type = PermissionType.Button, ParentId = 100, ParentIds = "0,1,100", Sort = 4, Status = true, CreateTime = now }

            };

            await _context.Permissions.AddRangeAsync(permissions);
            _logger.LogInformation("已添加 {Count} 个权限", permissions.Count);
        }

        /// <summary>
        /// 初始化用户角色关联
        /// </summary>
        private async Task SeedUserRolesAsync()
        {
            if (await _context.UserRoles.AnyAsync())
            {
                _logger.LogInformation("用户角色关联数据已存在，跳过初始化");
                return;
            }

            var userRoles = new List<UserRole>
            {
                new UserRole { Id = 1, UserId = 1, RoleId = 1, CreateTime = DateTime.Now }, // admin -> 超级管理员
                new UserRole { Id = 2, UserId = 2, RoleId = 2, CreateTime = DateTime.Now }, // manager -> 管理员
                new UserRole { Id = 3, UserId = 3, RoleId = 3, CreateTime = DateTime.Now }  // user -> 普通用户
            };

            await _context.UserRoles.AddRangeAsync(userRoles);
            _logger.LogInformation("已添加 {Count} 个用户角色关联", userRoles.Count);
        }

        /// <summary>
        /// 初始化角色权限关联
        /// </summary>
        private async Task SeedRolePermissionsAsync()
        {
            if (await _context.RolePermissions.AnyAsync())
            {
                _logger.LogInformation("角色权限关联数据已存在，跳过初始化");
                return;
            }

            var now = DateTime.Now;
            var rolePermissions = new List<RolePermission>
            {
                // 超级管理员拥有所有（目录/菜单/按钮）
                new RolePermission { Id = 1,  RoleId = 1, PermissionId = 1,  CreateTime = now },
                new RolePermission { Id = 2,  RoleId = 1, PermissionId = 2,  CreateTime = now },
                new RolePermission { Id = 3,  RoleId = 1, PermissionId = 3,  CreateTime = now },
                new RolePermission { Id = 4,  RoleId = 1, PermissionId = 4,  CreateTime = now },
                new RolePermission { Id = 5,  RoleId = 1, PermissionId = 5,  CreateTime = now },
                new RolePermission { Id = 6,  RoleId = 1, PermissionId = 6,  CreateTime = now },
                new RolePermission { Id = 7,  RoleId = 1, PermissionId = 7,  CreateTime = now },
                new RolePermission { Id = 8,  RoleId = 1, PermissionId = 8,  CreateTime = now },
                new RolePermission { Id = 9,  RoleId = 1, PermissionId = 9,  CreateTime = now },
                new RolePermission { Id = 10, RoleId = 1, PermissionId = 10, CreateTime = now },
                new RolePermission { Id = 11, RoleId = 1, PermissionId = 11, CreateTime = now },
                new RolePermission { Id = 12, RoleId = 1, PermissionId = 12, CreateTime = now },
                new RolePermission { Id = 13, RoleId = 1, PermissionId = 13, CreateTime = now },
                new RolePermission { Id = 14, RoleId = 1, PermissionId = 14, CreateTime = now },
                new RolePermission { Id = 15, RoleId = 1, PermissionId = 15, CreateTime = now },
                new RolePermission { Id = 16, RoleId = 1, PermissionId = 16, CreateTime = now },
                new RolePermission { Id = 17, RoleId = 1, PermissionId = 17, CreateTime = now },
                new RolePermission { Id = 18, RoleId = 1, PermissionId = 18, CreateTime = now },
                new RolePermission { Id = 19, RoleId = 1, PermissionId = 19, CreateTime = now },
                new RolePermission { Id = 20, RoleId = 1, PermissionId = 20, CreateTime = now },
                new RolePermission { Id = 21, RoleId = 1, PermissionId = 21, CreateTime = now },
                new RolePermission { Id = 301, RoleId = 1, PermissionId = 100, CreateTime = now },

                // 管理员：目录/菜单 + 常用按钮（查询/修改）
                new RolePermission { Id = 101, RoleId = 2, PermissionId = 1,  CreateTime = now },
                new RolePermission { Id = 102, RoleId = 2, PermissionId = 2,  CreateTime = now },
                new RolePermission { Id = 103, RoleId = 2, PermissionId = 3,  CreateTime = now },
                new RolePermission { Id = 104, RoleId = 2, PermissionId = 4,  CreateTime = now },
                new RolePermission { Id = 105, RoleId = 2, PermissionId = 5,  CreateTime = now },
                new RolePermission { Id = 106, RoleId = 2, PermissionId = 6,  CreateTime = now }, // user:list
                new RolePermission { Id = 107, RoleId = 2, PermissionId = 8,  CreateTime = now }, // user:update
                new RolePermission { Id = 108, RoleId = 2, PermissionId = 10, CreateTime = now }, // role:list
                new RolePermission { Id = 109, RoleId = 2, PermissionId = 12, CreateTime = now }, // role:update
                new RolePermission { Id = 110, RoleId = 2, PermissionId = 14, CreateTime = now }, // menu:list
                new RolePermission { Id = 111, RoleId = 2, PermissionId = 18, CreateTime = now }, // org:list

                // 普通用户：目录/菜单 + 只读按钮（查询）
                new RolePermission { Id = 201, RoleId = 3, PermissionId = 1,  CreateTime = now },
                new RolePermission { Id = 202, RoleId = 3, PermissionId = 2,  CreateTime = now },
                new RolePermission { Id = 203, RoleId = 3, PermissionId = 3,  CreateTime = now },
                new RolePermission { Id = 204, RoleId = 3, PermissionId = 5,  CreateTime = now },
                new RolePermission { Id = 205, RoleId = 3, PermissionId = 6,  CreateTime = now }, // user:list
                new RolePermission { Id = 206, RoleId = 3, PermissionId = 10, CreateTime = now }, // role:list
                new RolePermission { Id = 207, RoleId = 3, PermissionId = 14, CreateTime = now }  // menu:list
            };

            await _context.RolePermissions.AddRangeAsync(rolePermissions);
            _logger.LogInformation("已添加 {Count} 个角色权限关联", rolePermissions.Count);
        }
    }
}
