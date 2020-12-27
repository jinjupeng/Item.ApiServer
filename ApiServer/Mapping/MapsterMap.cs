
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using ApiServer.Model.Model.ViewModel;
using Mapster;

namespace ApiServer.Mapping
{
    /// <summary>
    /// 实体类映射
    /// </summary>
    public class MapsterMap
    {
        /// <summary>
        /// 
        /// </summary>
        public MapsterMap()
        {
            #region 返回前端实体类映射

            TypeAdapterConfig<Sys_Config, SysConfig>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            TypeAdapterConfig<Sys_Dict, SysDict>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            TypeAdapterConfig<Sys_Menu, SysMenuNode>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            TypeAdapterConfig<Sys_Api, SysApiNode>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            TypeAdapterConfig<Sys_Role, SysRole>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            TypeAdapterConfig<Sys_Org, SysOrgNode>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            #endregion

            #region 接收前端实体类映射

            TypeAdapterConfig<SysUser, Sys_User>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            TypeAdapterConfig<SysRole, Sys_Role>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            TypeAdapterConfig<SysOrg, Sys_Org>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            TypeAdapterConfig<SysMenu, Sys_Menu>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            TypeAdapterConfig<SysDict, Sys_Dict>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            TypeAdapterConfig<SysConfig, Sys_Config>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);
            TypeAdapterConfig<SysApi, Sys_Api>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.FromCamelCase);

            #endregion
        }

    }
}
