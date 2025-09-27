using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Infrastructure.Data;
using ApiServer.Infrastructure.Repositories;
using ApiServer.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiServer.Infrastructure
{
    /// <summary>
    /// 基础设施层依赖注入扩展
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 添加基础设施层服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 添加数据库上下文
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), builder =>
                {
                    builder.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });

                // 开发环境下启用敏感数据日志记录
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // 注册工作单元
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            // 注册仓储
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();

            // 注册应用服务
            services.AddScoped<Application.Interfaces.Services.IUserService, Application.Services.UserService>();
            services.AddScoped<Application.Interfaces.Services.IRoleService, Application.Services.RoleService>();
            services.AddScoped<Application.Interfaces.Services.IMenuService, Application.Services.MenuService>();
            services.AddScoped<Application.Interfaces.Services.IOrganizationService, Application.Services.OrganizationService>();
            services.AddScoped<Application.Interfaces.Services.IAuthService, Application.Services.AuthService>();

            // 注册基础设施服务
            RegisterInfrastructureServices(services);

            return services;
        }

        /// <summary>
        /// 注册基础设施服务
        /// </summary>
        /// <param name="services">服务集合</param>
        private static void RegisterInfrastructureServices(IServiceCollection services)
        {
            // 这里可以添加其他基础设施服务的注册
            // 例如：缓存服务、消息队列服务、文件存储服务等
            
            // 示例：
            // services.AddScoped<ICacheService, RedisCacheService>();
            // services.AddScoped<IFileStorageService, LocalFileStorageService>();
            // services.AddScoped<IEmailService, SmtpEmailService>();
        }

        /// <summary>
        /// 确保数据库已创建
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            try
            {
                // 确保数据库已创建
                await context.Database.EnsureCreatedAsync();
                
                // 或者使用迁移
                // await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                // 记录数据库初始化错误
                var logger = scope.ServiceProvider.GetService<Microsoft.Extensions.Logging.ILogger<ApplicationDbContext>>();
                logger?.LogError(ex, "An error occurred while ensuring the database was created.");
                throw;
            }
        }
    }
}