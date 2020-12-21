using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysConfigService
    {
        MsgModel GetSysConfigList();
        MsgModel QueryConfigs(string configLik);

        Sys_Config GetConfig(string paramKey);
        void UpdateConfig(Sys_Config sys_Config);

        void AddConfig(Sys_Config sys_Config);

        void DeleteConfig(long configId);
    }
}
