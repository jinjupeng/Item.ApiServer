using ApiServer.Application.Interfaces;
using ApiServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ApiServer.Infrastructure.Data
{
    /// <summary>
    /// 应用程序数据库上下文
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUser _currentUser;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUser currentUser) : base(options)
        {
            _currentUser = currentUser;
        }

        #region DbSets

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 应用所有配置
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // 配置表名
            ConfigureTableNames(modelBuilder);

            // 配置关系
            ConfigureRelationships(modelBuilder);

            // 配置索引
            ConfigureIndexes(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 配置表名
        /// </summary>
        private void ConfigureTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("sys_user");
            modelBuilder.Entity<Role>().ToTable("sys_role");
            modelBuilder.Entity<UserRole>().ToTable("sys_user_role");
            modelBuilder.Entity<Organization>().ToTable("sys_org");
            modelBuilder.Entity<Permission>().ToTable("sys_permission");
            modelBuilder.Entity<RolePermission>().ToTable("sys_role_permission");
            modelBuilder.Entity<AuditLog>().ToTable("sys_audit_log");
        }

        /// <summary>
        /// 配置关系
        /// </summary>
        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // 用户与组织的关系
            modelBuilder.Entity<User>()
                .HasOne(u => u.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(u => u.OrgId)
                .OnDelete(DeleteBehavior.Restrict);

            // 用户角色多对多关系
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 权限树形结构
            modelBuilder.Entity<Permission>()
                .HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 角色权限多对多关系
            modelBuilder.Entity<RolePermission>()
                .HasOne(ra => ra.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(ra => ra.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(ra => ra.Permission)
                .WithMany(a => a.RolePermissions)
                .HasForeignKey(ra => ra.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // 组织树形结构
            modelBuilder.Entity<Organization>()
                .HasOne(o => o.Parent)
                .WithMany(o => o.Children)
                .HasForeignKey(o => o.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// 配置索引
        /// </summary>
        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // 用户表索引
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique()
                .HasDatabaseName("IX_User_Username");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .HasDatabaseName("IX_User_Email");

            // 用户角色复合索引
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique()
                .HasDatabaseName("IX_UserRole_UserId_RoleId");

            // 角色菜单复合索引
            modelBuilder.Entity<RolePermission>()
                .HasIndex(rm => new { rm.RoleId, rm.PermissionId })
                .IsUnique()
                .HasDatabaseName("IX_RoleMenu_RoleId_MenuId");

            // 角色API复合索引
            modelBuilder.Entity<RolePermission>()
                .HasIndex(ra => new { ra.RoleId, ra.PermissionId })
                .IsUnique()
                .HasDatabaseName("IX_RoleApi_RoleId_PermissionId");

            // 审计日志索引
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.UserId)
                .HasDatabaseName("IX_AuditLog_UserId");
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.Action)
                .HasDatabaseName("IX_AuditLog_Action");
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.Module)
                .HasDatabaseName("IX_AuditLog_Module");
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.Result)
                .HasDatabaseName("IX_AuditLog_Result");
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.EntityType)
                .HasDatabaseName("IX_AuditLog_EntityType");
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.IpAddress)
                .HasDatabaseName("IX_AuditLog_IpAddress");
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.CreateTime)
                .HasDatabaseName("IX_AuditLog_CreateTime");
        }

        /// <summary>
        /// 保存更改（重写以支持审计）
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 在保存前处理审计字段
            HandleAuditFields();

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 处理审计字段
        /// </summary>
        private void HandleAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Domain.Common.BaseEntity && 
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is Domain.Common.BaseEntity baseEntity)
                    {
                        baseEntity.CreateTime = DateTime.Now;
                    }
                }

                if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is Domain.Common.AuditableEntity auditableEntity)
                    {
                        auditableEntity.LastModifiedTime = DateTime.Now;
                        // 从当前用户上下文获取用户ID
                        auditableEntity.LastModifiedBy = _currentUser.UserId;
                    }
                }
            }
        }
    }
}