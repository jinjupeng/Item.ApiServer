using ApiServer.BLL.IBLL;
using ApiServer.Common;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ApiServer.BLL.BLL
{
    public class SysConfigService : ISysConfigService
    {
        private readonly IMySystemService _mySystemService;
        private readonly IBaseService<Sys_Config> _baseSysConfigService;

        public SysConfigService(IBaseService<Sys_Config> baseSysConfigService)
        {
            _baseSysConfigService = baseSysConfigService;
        }

        public MsgModel GetSysConfigList()
        {
            MsgModel msg = new MsgModel
            {
                message = "查询成功！",
                isok = true,
                data = _baseSysConfigService.GetModels(null).ToList()
            };
            return msg;
        }

        public MsgModel QueryConfigs(string configLik)
        {
            MsgModel msg = new MsgModel()
            {
                isok = true,
                message = "查询成功！"
            };
            Expression<Func<Sys_Config, bool>> express = null;
            if (!string.IsNullOrEmpty(configLik))
            {
                express = a => a.param_name.Contains(configLik) || a.param_key.Contains(configLik);
            }
            msg.data = _baseSysConfigService.GetModels(express).ToList();
            return msg;
        }

        public Sys_Config GetConfig(string paramKey)
        {
            return _baseSysConfigService.GetModels(a => a.param_key == paramKey).ToList().SingleOrDefault();
        }

        public MsgModel UpdateConfig(Sys_Config sys_Config)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "更新配置成功！"
            };
            _baseSysConfigService.UpdateRange(sys_Config);
            return msg;
        }

        public MsgModel AddConfig(Sys_Config sys_Config)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "新增配置成功！"
            };
            sys_Config.id = new Snowflake().GetId();
            _baseSysConfigService.AddRange(sys_Config);
            return msg;
        }

        public MsgModel DeleteConfig(long configId)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "删除配置成功！"
            };
            _baseSysConfigService.DeleteRange(_baseSysConfigService.GetModels(a => a.id == configId));
            return msg;
        }
    }
}
