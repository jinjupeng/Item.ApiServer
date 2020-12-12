using ApiServer.Model.Entity;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysConfigService
    {
        List<Sys_Config> QueryConfigs(string configLik);
        void UpdateConfig(Sys_Config sys_Config);

        void AddConfig(Sys_Config sys_Config);

        void DeleteConfig(long configId);
    }
}
