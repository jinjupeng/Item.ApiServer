using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using System.Collections.Generic;
using System.Linq;

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

        public List<Sys_Config> GetSysConfigList()
        {
            List<Sys_Config> configList = _baseSysConfigService.GetModels(null).ToList();
            return configList;
        }

        public List<Sys_Config> QueryConfigs(string configLik)
        {
            List<Sys_Config> configList = new List<Sys_Config>();
            if (string.IsNullOrEmpty(configLik))
            {
                return configList;
            }
            configList = _baseSysConfigService.GetModels(a => a.param_name.Contains(configLik) || a.param_key.Contains(configLik)).ToList();
            return configList;
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
