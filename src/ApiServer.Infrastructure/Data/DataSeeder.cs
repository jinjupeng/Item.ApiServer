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

                // 初始化菜单数据（包含目录/菜单/按钮）
                await SeedMenusAsync();

                // 初始化权限数据（与按钮权限点对应）
                await SeedPermissionsAsync();

                // 初始化用户角色关联
                await SeedUserRolesAsync();

                // 初始化角色菜单关联（包含按钮级权限）
                await SeedRoleMenusAsync();

                // 初始化角色权限关联（API/按钮权限码）
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
                    Desc = "系统超级管理员，拥有所有权限",
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Role
                {
                    Id = 2,
                    Name = "管理员",
                    Desc = "系统管理员，拥有大部分权限",
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Role
                {
                    Id = 3,
                    Name = "普通用户",
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
                    Password = "E10ADC3949BA59ABBE56E057F20F883E", // 123456的MD5
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
                    Password = "E10ADC3949BA59ABBE56E057F20F883E", // 123456的MD5
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
                    Password = "E10ADC3949BA59ABBE56E057F20F883E", // 123456的MD5
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
        /// 初始化菜单数据（包含按钮权限点）
        /// </summary>
        private async Task SeedMenusAsync()
        {
            if (await _context.Menus.AnyAsync())
            {
                _logger.LogInformation("菜单数据已存在，跳过初始化");
                return;
            }

            var now = DateTime.Now;
            var menus = new List<Menu>
            {
                // 根目录：系统管理
                new Menu { Id = 1, Code = "SYSTEM", Name = "系统管理", Type = MenuType.Directory, Sort = 1, Status = true, Icon = "system", Url = "/system", CreateTime = now },

                // 一级菜单
                new Menu { Id = 2, Code = "USER_MANAGE", Name = "用户管理", Type = MenuType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 1, Status = true, Icon = "user", Url = "/system/users", CreateTime = now },
                new Menu { Id = 3, Code = "ROLE_MANAGE", Name = "角色管理", Type = MenuType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 2, Status = true, Icon = "role", Url = "/system/roles", CreateTime = now },
                new Menu { Id = 4, Code = "ORG_MANAGE",  Name = "组织管理", Type = MenuType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 3, Status = true, Icon = "org",  Url = "/system/org",   CreateTime = now },
                new Menu { Id = 5, Code = "MENU_MANAGE", Name = "菜单管理", Type = MenuType.Menu, ParentId = 1, ParentIds = "0,1", Sort = 4, Status = true, Icon = "menu", Url = "/system/menus", CreateTime = now },

                // 用户管理-按钮
                new Menu { Id = 6,  Code = "system:user:list",   Name = "用户查询", Type = MenuType.Button, ParentId = 2, ParentIds = "0,1,2", Sort = 1, Status = true, CreateTime = now },
                new Menu { Id = 7,  Code = "system:user:create", Name = "用户新增", Type = MenuType.Button, ParentId = 2, ParentIds = "0,1,2", Sort = 2, Status = true, CreateTime = now },
                new Menu { Id = 8,  Code = "system:user:update", Name = "用户修改", Type = MenuType.Button, ParentId = 2, ParentIds = "0,1,2", Sort = 3, Status = true, CreateTime = now },
                new Menu { Id = 9,  Code = "system:user:delete", Name = "用户删除", Type = MenuType.Button, ParentId = 2, ParentIds = "0,1,2", Sort = 4, Status = true, CreateTime = now },

                // 角色管理-按钮
                new Menu { Id = 10, Code = "system:role:list",   Name = "角色查询", Type = MenuType.Button, ParentId = 3, ParentIds = "0,1,3", Sort = 1, Status = true, CreateTime = now },
                new Menu { Id = 11, Code = "system:role:create", Name = "角色新增", Type = MenuType.Button, ParentId = 3, ParentIds = "0,1,3", Sort = 2, Status = true, CreateTime = now },
                new Menu { Id = 12, Code = "system:role:update", Name = "角色修改", Type = MenuType.Button, ParentId = 3, ParentIds = "0,1,3", Sort = 3, Status = true, CreateTime = now },
                new Menu { Id = 13, Code = "system:role:delete", Name = "角色删除", Type = MenuType.Button, ParentId = 3, ParentIds = "0,1,3", Sort = 4, Status = true, CreateTime = now },

                // 菜单管理-按钮
                new Menu { Id = 14, Code = "system:menu:list",   Name = "菜单查询", Type = MenuType.Button, ParentId = 5, ParentIds = "0,1,5", Sort = 1, Status = true, CreateTime = now },
                new Menu { Id = 15, Code = "system:menu:create", Name = "菜单新增", Type = MenuType.Button, ParentId = 5, ParentIds = "0,1,5", Sort = 2, Status = true, CreateTime = now },
                new Menu { Id = 16, Code = "system:menu:update", Name = "菜单修改", Type = MenuType.Button, ParentId = 5, ParentIds = "0,1,5", Sort = 3, Status = true, CreateTime = now },
                new Menu { Id = 17, Code = "system:menu:delete", Name = "菜单删除", Type = MenuType.Button, ParentId = 5, ParentIds = "0,1,5", Sort = 4, Status = true, CreateTime = now },

                // 组织管理-按钮
                new Menu { Id = 18, Code = "system:org:list",    Name = "组织查询", Type = MenuType.Button, ParentId = 4, ParentIds = "0,1,4", Sort = 1, Status = true, CreateTime = now },
                new Menu { Id = 19, Code = "system:org:create",  Name = "组织新增", Type = MenuType.Button, ParentId = 4, ParentIds = "0,1,4", Sort = 2, Status = true, CreateTime = now },
                new Menu { Id = 20, Code = "system:org:update",  Name = "组织修改", Type = MenuType.Button, ParentId = 4, ParentIds = "0,1,4", Sort = 3, Status = true, CreateTime = now },
                new Menu { Id = 21, Code = "system:org:delete",  Name = "组织删除", Type = MenuType.Button, ParentId = 4, ParentIds = "0,1,4", Sort = 4, Status = true, CreateTime = now }
            };

            await _context.Menus.AddRangeAsync(menus);
            _logger.LogInformation("已添加 {Count} 个菜单", menus.Count);
        }

        /// <summary>
        /// 初始化权限数据（与按钮权限点对应）
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
                // 用户管理权限
                new Permission { Id = 1,  Code = "system:user:list",   Name = "用户查询", Sort = 1,  Status = true, Url = "/api/user/query",  CreateTime = now },
                new Permission { Id = 2,  Code = "system:user:create", Name = "用户新增", Sort = 2,  Status = true, Url = "/api/user/create", CreateTime = now },
                new Permission { Id = 3,  Code = "system:user:update", Name = "用户修改", Sort = 3,  Status = true, Url = "/api/user/update", CreateTime = now },
                new Permission { Id = 4,  Code = "system:user:delete", Name = "用户删除", Sort = 4,  Status = true, Url = "/api/user/delete", CreateTime = now },

                // 角色管理权限
                new Permission { Id = 5,  Code = "system:role:list",   Name = "角色查询", Sort = 5,  Status = true, Url = "/api/role/query",  CreateTime = now },
                new Permission { Id = 6,  Code = "system:role:create", Name = "角色新增", Sort = 6,  Status = true, Url = "/api/role/create", CreateTime = now },
                new Permission { Id = 7,  Code = "system:role:update", Name = "角色修改", Sort = 7,  Status = true, Url = "/api/role/update", CreateTime = now },
                new Permission { Id = 8,  Code = "system:role:delete", Name = "角色删除", Sort = 8,  Status = true, Url = "/api/role/delete", CreateTime = now },

                // 菜单管理权限
                new Permission { Id = 9,  Code = "system:menu:list",   Name = "菜单查询", Sort = 9,  Status = true, Url = "/api/menu/query",  CreateTime = now },
                new Permission { Id = 10, Code = "system:menu:create", Name = "菜单新增", Sort = 10, Status = true, Url = "/api/menu/create", CreateTime = now },
                new Permission { Id = 11, Code = "system:menu:update", Name = "菜单修改", Sort = 11, Status = true, Url = "/api/menu/update", CreateTime = now },
                new Permission { Id = 12, Code = "system:menu:delete", Name = "菜单删除", Sort = 12, Status = true, Url = "/api/menu/delete", CreateTime = now },

                // 组织管理权限
                new Permission { Id = 13, Code = "system:org:list",    Name = "组织查询", Sort = 13, Status = true, Url = "/api/org/query",   CreateTime = now },
                new Permission { Id = 14, Code = "system:org:create",  Name = "组织新增", Sort = 14, Status = true, Url = "/api/org/create",  CreateTime = now },
                new Permission { Id = 15, Code = "system:org:update",  Name = "组织修改", Sort = 15, Status = true, Url = "/api/org/update",  CreateTime = now },
                new Permission { Id = 16, Code = "system:org:delete",  Name = "组织删除", Sort = 16, Status = true, Url = "/api/org/delete",  CreateTime = now }
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
        /// 初始化角色菜单关联（包含按钮）
        /// </summary>
        private async Task SeedRoleMenusAsync()
        {
            if (await _context.RoleMenus.AnyAsync())
            {
                _logger.LogInformation("角色菜单关联数据已存在，跳过初始化");
                return;
            }

            var now = DateTime.Now;
            var roleMenus = new List<RoleMenu>
            {
                // 超级管理员拥有所有（目录/菜单/按钮）
                new RoleMenu { Id = 1,  RoleId = 1, MenuId = 1,  CreateTime = now },
                new RoleMenu { Id = 2,  RoleId = 1, MenuId = 2,  CreateTime = now },
                new RoleMenu { Id = 3,  RoleId = 1, MenuId = 3,  CreateTime = now },
                new RoleMenu { Id = 4,  RoleId = 1, MenuId = 4,  CreateTime = now },
                new RoleMenu { Id = 5,  RoleId = 1, MenuId = 5,  CreateTime = now },
                new RoleMenu { Id = 6,  RoleId = 1, MenuId = 6,  CreateTime = now },
                new RoleMenu { Id = 7,  RoleId = 1, MenuId = 7,  CreateTime = now },
                new RoleMenu { Id = 8,  RoleId = 1, MenuId = 8,  CreateTime = now },
                new RoleMenu { Id = 9,  RoleId = 1, MenuId = 9,  CreateTime = now },
                new RoleMenu { Id = 10, RoleId = 1, MenuId = 10, CreateTime = now },
                new RoleMenu { Id = 11, RoleId = 1, MenuId = 11, CreateTime = now },
                new RoleMenu { Id = 12, RoleId = 1, MenuId = 12, CreateTime = now },
                new RoleMenu { Id = 13, RoleId = 1, MenuId = 13, CreateTime = now },
                new RoleMenu { Id = 14, RoleId = 1, MenuId = 14, CreateTime = now },
                new RoleMenu { Id = 15, RoleId = 1, MenuId = 15, CreateTime = now },
                new RoleMenu { Id = 16, RoleId = 1, MenuId = 16, CreateTime = now },
                new RoleMenu { Id = 17, RoleId = 1, MenuId = 17, CreateTime = now },
                new RoleMenu { Id = 18, RoleId = 1, MenuId = 18, CreateTime = now },
                new RoleMenu { Id = 19, RoleId = 1, MenuId = 19, CreateTime = now },
                new RoleMenu { Id = 20, RoleId = 1, MenuId = 20, CreateTime = now },
                new RoleMenu { Id = 21, RoleId = 1, MenuId = 21, CreateTime = now },

                // 管理员：目录/菜单 + 常用按钮（查询/修改）
                new RoleMenu { Id = 101, RoleId = 2, MenuId = 1,  CreateTime = now },
                new RoleMenu { Id = 102, RoleId = 2, MenuId = 2,  CreateTime = now },
                new RoleMenu { Id = 103, RoleId = 2, MenuId = 3,  CreateTime = now },
                new RoleMenu { Id = 104, RoleId = 2, MenuId = 4,  CreateTime = now },
                new RoleMenu { Id = 105, RoleId = 2, MenuId = 5,  CreateTime = now },
                new RoleMenu { Id = 106, RoleId = 2, MenuId = 6,  CreateTime = now }, // user:list
                new RoleMenu { Id = 107, RoleId = 2, MenuId = 8,  CreateTime = now }, // user:update
                new RoleMenu { Id = 108, RoleId = 2, MenuId = 10, CreateTime = now }, // role:list
                new RoleMenu { Id = 109, RoleId = 2, MenuId = 12, CreateTime = now }, // role:update
                new RoleMenu { Id = 110, RoleId = 2, MenuId = 14, CreateTime = now }, // menu:list
                new RoleMenu { Id = 111, RoleId = 2, MenuId = 18, CreateTime = now }, // org:list

                // 普通用户：目录/菜单 + 只读按钮（查询）
                new RoleMenu { Id = 201, RoleId = 3, MenuId = 1,  CreateTime = now },
                new RoleMenu { Id = 202, RoleId = 3, MenuId = 2,  CreateTime = now },
                new RoleMenu { Id = 203, RoleId = 3, MenuId = 3,  CreateTime = now },
                new RoleMenu { Id = 204, RoleId = 3, MenuId = 5,  CreateTime = now },
                new RoleMenu { Id = 205, RoleId = 3, MenuId = 6,  CreateTime = now }, // user:list
                new RoleMenu { Id = 206, RoleId = 3, MenuId = 10, CreateTime = now }, // role:list
                new RoleMenu { Id = 207, RoleId = 3, MenuId = 14, CreateTime = now }  // menu:list
            };

            await _context.RoleMenus.AddRangeAsync(roleMenus);
            _logger.LogInformation("已添加 {Count} 个角色菜单关联", roleMenus.Count);
        }

        /// <summary>
        /// 初始化角色权限关联（API权限）
        /// </summary>
        private async Task SeedRolePermissionsAsync()
        {
            if (await _context.RolePermissions.AnyAsync())
            {
                _logger.LogInformation("角色权限关联数据已存在，跳过初始化");
                return;
            }

            var now = DateTime.Now;
            var rolePermissions = new List<RolePermission>();

            // 超级管理员：所有权限
            for (int pid = 1; pid <= 16; pid++)
            {
                rolePermissions.Add(new RolePermission { Id = pid, RoleId = 1, PermissionId = pid, CreateTime = now });
            }

            // 管理员：查询+修改
            var adminPermIds = new[] { 1, 3, 5, 7, 9, 13 }; // user:list, user:update, role:list, role:update, menu:list, org:list
            int rid = 1000;
            foreach (var pid in adminPermIds)
            {
                rolePermissions.Add(new RolePermission { Id = rid++, RoleId = 2, PermissionId = pid, CreateTime = now });
            }

            // 普通用户：只读
            var userPermIds = new[] { 1, 5, 9, 13 };
            foreach (var pid in userPermIds)
            {
                rolePermissions.Add(new RolePermission { Id = rid++, RoleId = 3, PermissionId = pid, CreateTime = now });
            }

            await _context.RolePermissions.AddRangeAsync(rolePermissions);
            _logger.LogInformation("已添加 {Count} 个角色权限关联", rolePermissions.Count);
        }
    }
}
