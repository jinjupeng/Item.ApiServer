using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using ApiServer.Model.Model.ViewModel;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ApiServer.Extensions.Mapping
{
    /// <summary>
    /// Mapster注入
    /// </summary>
    public static class MapsterMap
    {
        /// <summary>
        /// 自定义扩展service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMapster(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;

            #region 返回前端实体类映射

            config.NewConfig<Sys_Config, SysConfig>().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            config.NewConfig<Sys_Dict, SysDict>().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            config.NewConfig<Sys_Menu, SysMenuNode>().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            config.NewConfig<Sys_Api, SysApiNode>().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            config.NewConfig<Sys_Role, SysRole>().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            config.NewConfig<Sys_Org, SysOrgNode>().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            #endregion

            #region 接收前端实体类映射

            config.NewConfig<SysUser, Sys_User>().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            config.NewConfig<SysRole, Sys_Role>().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            config.NewConfig<SysOrg, Sys_Org>().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            config.NewConfig<SysMenu, Sys_Menu>().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            config.NewConfig<SysDict, Sys_Dict>().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            config.NewConfig<SysConfig, Sys_Config>().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            config.NewConfig<SysApi, Sys_Api>().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);

            #endregion

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}
