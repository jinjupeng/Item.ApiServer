using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;

namespace ApiServer.BLL.IBLL
{
    public interface ISysConfigService
    {
        MsgModel GetSysConfigList();
        MsgModel QueryConfigs(string configLik);

        string GetConfigItem(string paramKey);
        MsgModel UpdateConfig(Sys_Config sys_Config);

        MsgModel AddConfig(Sys_Config sys_Config);

        MsgModel DeleteConfig(long configId);
    }
}
