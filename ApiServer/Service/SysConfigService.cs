using ApiServer.Models.Entity;
using ApiServer.Models.Model.MsgModel;
using ApiServer.Models.Model.ViewModel;
using Common.Utility.Utils;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApiServer.BLL.BLL
{
    public interface ISysConfigService
    {
        MsgModel GetSysConfigList();
        MsgModel QueryConfigs(string configLik);

        string GetConfigItem(string paramKey);
        MsgModel UpdateConfig(sys_config sys_Config);

        MsgModel AddConfig(sys_config sys_Config);

        MsgModel DeleteConfig(long configId);
    }
    public class SysConfigService : ISysConfigService
    {
        private readonly IBaseService<sys_config> _baseSysConfigService;

        public SysConfigService(IBaseService<sys_config> baseSysConfigService)
        {
            _baseSysConfigService = baseSysConfigService;
        }

        public MsgModel GetSysConfigList()
        {
            List<sys_config> list = _baseSysConfigService.GetModels(null).ToList();
            var data = list.BuildAdapter().AdaptToType<List<SysConfig>>();
            return MsgModel.Success(data, "查询成功！");
        }

        public MsgModel QueryConfigs(string configLik)
        {
            Expression<Func<sys_config, bool>> express = null;
            if (!string.IsNullOrEmpty(configLik))
            {
                express = a => a.param_name.Contains(configLik) || a.param_key.Contains(configLik);
            }
            List<sys_config> list = _baseSysConfigService.GetModels(express).ToList();
            var data = list.BuildAdapter().AdaptToType<List<SysConfig>>();
            return MsgModel.Success(data, "查询成功！");
        }

        public string GetConfigItem(string paramKey)
        {
            return _baseSysConfigService.GetModels(a => a.param_key == paramKey).ToList().SingleOrDefault()?.param_value;
        }

        public MsgModel UpdateConfig(sys_config sys_Config)
        {
            var result = _baseSysConfigService.UpdateRange(sys_Config);
            return result ? MsgModel.Success("更新配置成功！") : MsgModel.Fail("更新配置失败！");
        }

        public MsgModel AddConfig(sys_config sys_Config)
        {
            sys_Config.id = new Snowflake().GetId();
            var result = _baseSysConfigService.AddRange(sys_Config);
            return result ? MsgModel.Success("新增配置成功！") : MsgModel.Fail("新增配置失败！");
        }

        public MsgModel DeleteConfig(long configId)
        {
            var result = _baseSysConfigService.DeleteRange(_baseSysConfigService.GetModels(a => a.id == configId));
            return result ? MsgModel.Success("删除配置成功！") : MsgModel.Fail("删除配置失败！");
        }
    }
}
