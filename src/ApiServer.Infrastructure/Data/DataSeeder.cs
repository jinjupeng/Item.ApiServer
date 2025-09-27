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

                // 初始化菜单数据
                await SeedMenusAsync();

                // 初始化权限数据
                await SeedPermissionsAsync();

                // 初始化用户角色关联
                await SeedUserRolesAsync();

                // 初始化角色菜单关联
                await SeedRoleMenusAsync();

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
                    IsLeaf = false,
                    Sort = 1,
                    Level = 1,
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
                    IsLeaf = true,
                    Sort = 1,
                    Level = 2,
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
                    IsLeaf = true,
                    Sort = 2,
                    Level = 2,
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
                    IsLeaf = true,
                    Sort = 3,
                    Level = 2,
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
                    RoleName = "超级管理员",
                    RoleDesc = "系统超级管理员，拥有所有权限",
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Role
                {
                    Id = 2,
                    RoleName = "管理员",
                    RoleDesc = "系统管理员，拥有大部分权限",
                    Status = true,
                    CreateTime = DateTime.Now
                },
                new Role
                {
                    Id = 3,
                    RoleName = "普通用户",
                    RoleDesc = "普通用户，拥有基本权限",
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
                    Username = "admin",
                    Password = "E10ADC3949BA59ABBE56E057F20F883E", // 123456的MD5
                    Nickname = "超级管理员",
                    OrgId = 1,
                    Status = UserStatus.Enabled,
                    Email = "admin@example.com",
                    Phone = "13800138000",
                    CreateTime = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Username = "manager",
                    Password = "E10ADC3949BA59ABBE56E057F20F883E", // 123456的MD5
                    Nickname = "部门经理",
                    OrgId = 2,
                    Status = UserStatus.Enabled,
                    Email = "manager@example.com",
                    Phone = "13800138001",
                    CreateTime = DateTime.Now
                },
                new User
                {
                    Id = 3,
                    Username = "user",
                    Password = "E10ADC3949BA59ABBE56E057F20F883E", // 123456的MD5
                    Nickname = "普通用户",
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
        /// 初始化菜单数据
        /// </summary>
        private async Task SeedMenusAsync()
        {
            if (await _context.Menus.AnyAsync())
            {
                _logger.LogInformation("菜单数据已存在，跳过初始化");
                return;
            }

            var menus = new List<Menu>
            {
                // 系统管理
                new Menu
                {
                    Id = 1,
                    Code = "SYSTEM",
                    Name = "系统管理",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = false,
                    Level = 1,
                    Sort = 1,
                    Status = true,
                    Icon = "system",
                    Url = "/system",
                    CreateTime = DateTime.Now
                },
                // 用户管理
                new Menu
                {
                    Id = 2,
                    Code = "USER_MANAGE",
                    Name = "用户管理",
                    ParentId = 1,
                    ParentIds = "0,1",
                    IsLeaf = true,
                    Level = 2,
                    Sort = 1,
                    Status = true,
                    Icon = "user",
                    Url = "/system/user",
                    CreateTime = DateTime.Now
                },
                // 角色管理
                new Menu
                {
                    Id = 3,
                    Code = "ROLE_MANAGE",
                    Name = "角色管理",
                    ParentId = 1,
                    ParentIds = "0,1",
                    IsLeaf = true,
                    Level = 2,
                    Sort = 2,
                    Status = true,
                    Icon = "role",
                    Url = "/system/role",
                    CreateTime = DateTime.Now
                },
                // 组织管理
                new Menu
                {
                    Id = 4,
                    Code = "ORG_MANAGE",
                    Name = "组织管理",
                    ParentId = 1,
                    ParentIds = "0,1",
                    IsLeaf = true,
                    Level = 2,
                    Sort = 3,
                    Status = true,
                    Icon = "org",
                    Url = "/system/org",
                    CreateTime = DateTime.Now
                },
                // 菜单管理
                new Menu
                {
                    Id = 5,
                    Code = "MENU_MANAGE",
                    Name = "菜单管理",
                    ParentId = 1,
                    ParentIds = "0,1",
                    IsLeaf = true,
                    Level = 2,
                    Sort = 4,
                    Status = true,
                    Icon = "menu",
                    Url = "/system/menu",
                    CreateTime = DateTime.Now
                }
            };

            await _context.Menus.AddRangeAsync(menus);
            _logger.LogInformation("已添加 {Count} 个菜单", menus.Count);
        }

        /// <summary>
        /// 初始化权限数据
        /// </summary>
        private async Task SeedPermissionsAsync()
        {
            if (await _context.Permissions.AnyAsync())
            {
                _logger.LogInformation("权限数据已存在，跳过初始化");
                return;
            }

            var permissions = new List<Permission>
            {
                // 用户管理权限
                new Permission
                {
                    Id = 1,
                    Name = "用户查询",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = true,
                    Level = 1,
                    Sort = 1,
                    Status = true,
                    Url = "/api/user/query",
                    CreateTime = DateTime.Now
                },
                new Permission
                {
                    Id = 2,
                    Name = "用户新增",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = true,
                    Level = 1,
                    Sort = 2,
                    Status = true,
                    Url = "/api/user/create",
                    CreateTime = DateTime.Now
                },
                new Permission
                {
                    Id = 3,
                    Name = "用户修改",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = true,
                    Level = 1,
                    Sort = 3,
                    Status = true,
                    Url = "/api/user/update",
                    CreateTime = DateTime.Now
                },
                new Permission
                {
                    Id = 4,
                    Name = "用户删除",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = true,
                    Level = 1,
                    Sort = 4,
                    Status = true,
                    Url = "/api/user/delete",
                    CreateTime = DateTime.Now
                },
                // 角色管理权限
                new Permission
                {
                    Id = 5,
                    Name = "角色查询",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = true,
                    Level = 1,
                    Sort = 5,
                    Status = true,
                    Url = "/api/role/query",
                    CreateTime = DateTime.Now
                },
                new Permission
                {
                    Id = 6,
                    Name = "角色新增",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = true,
                    Level = 1,
                    Sort = 6,
                    Status = true,
                    Url = "/api/role/create",
                    CreateTime = DateTime.Now
                },
                new Permission
                {
                    Id = 7,
                    Name = "角色修改",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = true,
                    Level = 1,
                    Sort = 7,
                    Status = true,
                    Url = "/api/role/update",
                    CreateTime = DateTime.Now
                },
                new Permission
                {
                    Id = 8,
                    Name = "角色删除",
                    ParentId = null,
                    ParentIds = "0",
                    IsLeaf = true,
                    Level = 1,
                    Sort = 8,
                    Status = true,
                    Url = "/api/role/delete",
                    CreateTime = DateTime.Now
                }
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
        /// 初始化角色菜单关联
        /// </summary>
        private async Task SeedRoleMenusAsync()
        {
            if (await _context.RoleMenus.AnyAsync())
            {
                _logger.LogInformation("角色菜单关联数据已存在，跳过初始化");
                return;
            }

            var roleMenus = new List<RoleMenu>
            {
                // 超级管理员拥有所有菜单
                new RoleMenu { Id = 1, RoleId = 1, MenuId = 1, CreateTime = DateTime.Now },
                new RoleMenu { Id = 2, RoleId = 1, MenuId = 2, CreateTime = DateTime.Now },
                new RoleMenu { Id = 3, RoleId = 1, MenuId = 3, CreateTime = DateTime.Now },
                new RoleMenu { Id = 4, RoleId = 1, MenuId = 4, CreateTime = DateTime.Now },
                new RoleMenu { Id = 5, RoleId = 1, MenuId = 5, CreateTime = DateTime.Now },
                
                // 管理员拥有部分菜单
                new RoleMenu { Id = 6, RoleId = 2, MenuId = 1, CreateTime = DateTime.Now },
                new RoleMenu { Id = 7, RoleId = 2, MenuId = 2, CreateTime = DateTime.Now },
                new RoleMenu { Id = 8, RoleId = 2, MenuId = 4, CreateTime = DateTime.Now },
                
                // 普通用户只有基本菜单
                new RoleMenu { Id = 9, RoleId = 3, MenuId = 1, CreateTime = DateTime.Now },
                new RoleMenu { Id = 10, RoleId = 3, MenuId = 2, CreateTime = DateTime.Now }
            };

            await _context.RoleMenus.AddRangeAsync(roleMenus);
            _logger.LogInformation("已添加 {Count} 个角色菜单关联", roleMenus.Count);
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

            var rolePermissions = new List<RolePermission>
            {
                // 超级管理员拥有所有权限
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1, CreateTime = DateTime.Now },
                new RolePermission { Id = 2, RoleId = 1, PermissionId = 2, CreateTime = DateTime.Now },
                new RolePermission { Id = 3, RoleId = 1, PermissionId = 3, CreateTime = DateTime.Now },
                new RolePermission { Id = 4, RoleId = 1, PermissionId = 4, CreateTime = DateTime.Now },
                new RolePermission { Id = 5, RoleId = 1, PermissionId = 5, CreateTime = DateTime.Now },
                new RolePermission { Id = 6, RoleId = 1, PermissionId = 6, CreateTime = DateTime.Now },
                new RolePermission { Id = 7, RoleId = 1, PermissionId = 7, CreateTime = DateTime.Now },
                new RolePermission { Id = 8, RoleId = 1, PermissionId = 8, CreateTime = DateTime.Now },
                
                // 管理员拥有查询和修改权限
                new RolePermission { Id = 9, RoleId = 2, PermissionId = 1, CreateTime = DateTime.Now },
                new RolePermission { Id = 10, RoleId = 2, PermissionId = 3, CreateTime = DateTime.Now },
                new RolePermission { Id = 11, RoleId = 2, PermissionId = 5, CreateTime = DateTime.Now },
                new RolePermission { Id = 12, RoleId = 2, PermissionId = 7, CreateTime = DateTime.Now },
                
                // 普通用户只有查询权限
                new RolePermission { Id = 13, RoleId = 3, PermissionId = 1, CreateTime = DateTime.Now },
                new RolePermission { Id = 14, RoleId = 3, PermissionId = 5, CreateTime = DateTime.Now }
            };

            await _context.RolePermissions.AddRangeAsync(rolePermissions);
            _logger.LogInformation("已添加 {Count} 个角色权限关联", rolePermissions.Count);
        }
    }
}
