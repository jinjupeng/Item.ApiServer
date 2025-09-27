using ApiServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiServer.Infrastructure.Extensions
{
    /// <summary>
    /// 数据库扩展方法
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// 自动执行数据库迁移和初始化
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        public static async Task AutoMigrateAndInitializeAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseMigration");

            try
            {
                logger.LogInformation("开始自动数据库迁移和初始化...");

                // 检查数据库是否存在
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    logger.LogInformation("数据库不存在，将创建数据库");
                }

                // 获取待处理的迁移
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    logger.LogInformation("发现 {Count} 个待处理的迁移，开始自动应用...", pendingMigrations.Count());
                    foreach (var migration in pendingMigrations)
                    {
                        logger.LogInformation("待应用迁移: {Migration}", migration);
                    }
                    
                    // 自动应用迁移
                    await context.Database.MigrateAsync();
                    logger.LogInformation("数据库迁移完成");
                }
                else
                {
                    logger.LogInformation("数据库已是最新版本，无需迁移");
                    
                    // 如果没有迁移，确保数据库已创建
                    var created = await context.Database.EnsureCreatedAsync();
                    if (created)
                    {
                        logger.LogInformation("数据库已创建");
                    }
                }

                // 初始化种子数据
                await SeedDataAsync(serviceProvider);

                logger.LogInformation("数据库迁移和初始化全部完成");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "数据库迁移或初始化失败");
                throw;
            }
        }

        /// <summary>
        /// 确保数据库已创建并初始化种子数据（兼容性方法）
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
        {
            await AutoMigrateAndInitializeAsync(serviceProvider);
        }

        /// <summary>
        /// 初始化种子数据
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();

            var seeder = new DataSeeder(context, logger);
            await seeder.SeedAsync();
        }

        /// <summary>
        /// 重置数据库（删除并重新创建）
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        public static async Task ResetDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseMigration");

            try
            {
                logger.LogWarning("开始重置数据库...");

                // 删除数据库
                await context.Database.EnsureDeletedAsync();
                logger.LogInformation("数据库已删除");

                // 重新创建数据库
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("数据库已重新创建");

                // 初始化种子数据
                await SeedDataAsync(serviceProvider);

                logger.LogInformation("数据库重置完成");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "数据库重置失败");
                throw;
            }
        }

        /// <summary>
        /// 检查数据库连接
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        public static async Task<bool> CheckDatabaseConnectionAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseMigration");

            try
            {
                logger.LogInformation("检查数据库连接...");
                await context.Database.OpenConnectionAsync();
                await context.Database.CloseConnectionAsync();
                logger.LogInformation("数据库连接正常");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "数据库连接失败");
                return false;
            }
        }
    }
}
