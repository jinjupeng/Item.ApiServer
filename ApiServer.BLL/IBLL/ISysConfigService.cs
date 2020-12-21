using ApiServer.Model.Entity;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysConfigService
    {
        List<Sys_Config> GetSysConfigList();
        List<Sys_Config> QueryConfigs(string configLik);

        Sys_Config GetConfig(string paramKey);
        void UpdateConfig(Sys_Config sys_Config);

        void AddConfig(Sys_Config sys_Config);

        void DeleteConfig(long configId);
    }
}
