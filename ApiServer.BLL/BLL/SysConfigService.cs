using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System;
using System.Collections.Generic;
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
                isok = true
            };
            msg.data = _baseSysConfigService.GetModels(null).ToList();
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

        public void UpdateConfig(Sys_Config sys_Config)
        {
            _baseSysConfigService.UpdateRange(sys_Config);
        }

        public void AddConfig(Sys_Config sys_Config)
        {
            _baseSysConfigService.AddRange(sys_Config);
        }

        public void DeleteConfig(long configId)
        {
            _baseSysConfigService.DeleteRange(_baseSysConfigService.GetModels(a => a.id == configId));
        }
    }
}
