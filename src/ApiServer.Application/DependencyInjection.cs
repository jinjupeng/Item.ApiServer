// using ApiServer.Application.Interfaces.Services;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ApiServer.Application
{
    /// <summary>
    /// 应用层依赖注入扩展
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// 添加应用层服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 添加Mapster配置
            services.AddMapster();

            // 添加MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            // 注册应用服务
            RegisterApplicationServices(services);

            return services;
        }

        /// <summary>
        /// 注册应用服务
        /// </summary>
        /// <param name="services">服务集合</param>
        private static void RegisterApplicationServices(IServiceCollection services)
        {
            // 注册应用服务接口及其实现
            // 注意：实现类在Infrastructure层，服务注册在Infrastructure的DependencyInjection中完成
        }

        /// <summary>
        /// 添加Mapster配置
        /// </summary>
        /// <param name="services">服务集合</param>
        private static IServiceCollection AddMapster(this IServiceCollection services)
        {
            // 配置Mapster映射规则
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            
            // 配置特定的映射规则
            ConfigureMappings(config);
            
            return services;
        }

        /// <summary>
        /// 配置实体映射规则
        /// </summary>
        /// <param name="config">配置对象</param>
        private static void ConfigureMappings(TypeAdapterConfig config)
        {
            // 示例：配置User实体与DTO之间的映射
            // config.NewConfig<User, UserDto>()
            //       .Map(dest => dest.OrgName, src => src.Organization.OrgName)
            //       .Map(dest => dest.RoleNames, src => src.UserRoles.Select(ur => ur.Role.RoleName));
        }
    }
}