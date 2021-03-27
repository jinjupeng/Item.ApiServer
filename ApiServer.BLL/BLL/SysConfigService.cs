﻿using ApiServer.BLL.IBLL;
using ApiServer.Common;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using ApiServer.Model.Model.ViewModel;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApiServer.BLL.BLL
{
    public class SysConfigService : ISysConfigService
    {
        private readonly IBaseService<Sys_Config> _baseSysConfigService;

        public SysConfigService(IBaseService<Sys_Config> baseSysConfigService)
        {
            _baseSysConfigService = baseSysConfigService;
        }

        public MsgModel GetSysConfigList()
        {
            //TypeAdapterConfig<Sys_Config, SysConfig>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            List<Sys_Config> list = _baseSysConfigService.GetModels(null).ToList();
            var data = list.BuildAdapter().AdaptToType<List<SysConfig>>();
            return MsgModel.Success(data, "查询成功！");
        }

        public MsgModel QueryConfigs(string configLik)
        {
            Expression<Func<Sys_Config, bool>> express = null;
            if (!string.IsNullOrEmpty(configLik))
            {
                express = a => a.param_name.Contains(configLik) || a.param_key.Contains(configLik);
            }
            //TypeAdapterConfig<Sys_Config, SysConfig>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            List<Sys_Config> list = _baseSysConfigService.GetModels(express).ToList();
            var data = list.BuildAdapter().AdaptToType<List<SysConfig>>();
            return MsgModel.Success(data, "查询成功！");
        }

        public string GetConfigItem(string paramKey)
        {
            return _baseSysConfigService.GetModels(a => a.param_key == paramKey).ToList().SingleOrDefault()?.param_value;
        }

        public MsgModel UpdateConfig(Sys_Config sys_Config)
        {
            var result = _baseSysConfigService.UpdateRange(sys_Config);
            return result ? MsgModel.Success("更新配置成功！") : MsgModel.Fail("更新配置失败！");
        }

        public MsgModel AddConfig(Sys_Config sys_Config)
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
