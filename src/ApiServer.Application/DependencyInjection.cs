using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Application.Services;
using ApiServer.Application.Interfaces;

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
            // 应用服务接口与实现注册（作用域生命周期）
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
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

            ConfigureMappings(config);

            return services;
        }

        /// <summary>
        /// </summary>
        /// <param name="config">配置对象</param>
        private static void ConfigureMappings(TypeAdapterConfig config)
        {
            // 配置AuditLog实体与SystemAuditLogDto之间的映射
            config.NewConfig<Domain.Entities.AuditLog, DTOs.Audit.AuditLogDto>()
                .Map(dest => dest.Id, src => src.Id.ToString())
                .Map(dest => dest.CreatedAt, src => src.CreateTime);

            // 配置User实体与UserDto之间的映射
            config.NewConfig<Domain.Entities.User, DTOs.User.UserDto>()
                  .Map(dest => dest.Id, src => src.Id)
                  .Map(dest => dest.UserName, src => src.Name)
                  .Map(dest => dest.NickName, src => src.NickName)
                  .Map(dest => dest.Email, src => src.Email)
                  .Map(dest => dest.Phone, src => src.Phone)
                  .Map(dest => dest.Status, src => src.Status)
                  .Map(dest => dest.CreatedAt, src => src.CreateTime)
                  .Map(dest => dest.OrgName, src => src.Organization != null ? src.Organization.Name : "")
                  .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => new DTOs.Role.BaseRoleDto 
                  { 
                      Id = ur.Role.Id, 
                      Name = ur.Role.Name, 
                      Code = ur.Role.Code ?? ""
                  }).ToList());

            // 配置Menu实体与MenuTreeDto之间的映射
            config.NewConfig<Domain.Entities.Permission, DTOs.Menu.MenuTreeDto>()
                  .Map(dest => dest.MenuName, src => src.Name)
                  .Map(dest => dest.MenuCode, src => src.Code)
                  .Map(dest => dest.Icon, src => src.Icon)
                  .Map(dest => dest.ParentId, src => src.ParentId)
                  .Map(dest => dest.Sort, src => src.Sort)
                  .Map(dest => dest.MenuType, src => src.Type)
                  .Map(dest => dest.Expanded, src => false)
                  .Map(dest => dest.Status, src => src.Status)
                  .Map(dest => dest.Checked, src => false);


            // 配置Role实体与RoleDto之间的映射
            config.NewConfig<Domain.Entities.Role, DTOs.Role.RoleDto>()
                  .Map(dest => dest.Id, src => src.Id)
                  .Map(dest => dest.Name, src => src.Name)
                  .Map(dest => dest.Code, src => src.Code)
                  .Map(dest => dest.Description, src => src.Desc)
                  .Map(dest => dest.IsActive, src => src.Status)
                  .Map(dest => dest.CreateTime, src => src.CreateTime)
                  .Map(dest => dest.Sort, src => src.Sort);


            // 配置Permission实体与MenuDto之间的映射
            config.NewConfig<Domain.Entities.Permission, DTOs.Menu.MenuDto>()
                  .Map(dest => dest.Id, src => src.Id)
                  .Map(dest => dest.MenuName, src => src.Name)
                  .Map(dest => dest.MenuCode, src => src.Code)
                  .Map(dest => dest.MenuPid, src => src.ParentId)
                  .Map(dest => dest.MenuPids, src => src.ParentIds)
                  .Map(dest => dest.Status, src => src.Status)
                  .Map(dest => dest.CreateTime, src => src.CreateTime);

            // 示例：配置User实体与DTO之间的映射
            // config.NewConfig<User, UserDto>()
            //       .Map(dest => dest.OrgName, src => src.Organization.OrgName)
            //       .Map(dest => dest.RoleNames, src => src.UserRoles.Select(ur => ur.Role.RoleName));
        }
    }
}