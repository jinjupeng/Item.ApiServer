using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using Item.ApiServer.BLL.BLL;
using Item.ApiServer.BLL.IBLL;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public class SysConfigService : BaseService<Sys_Config>, ISysConfigService
    {
        private readonly IMySystemService _mySystemService;
        private readonly IBaseService<Sys_Config> _baseSysConfigService;

        public SysConfigService(IBaseService<Sys_Config> baseSysConfigService)
        {
            _baseSysConfigService = baseSysConfigService;
        }

        public List<Sys_Config> QueryConfigs(string configLik)
        {
            if (string.IsNullOrEmpty(configLik))
            {
                return null;
            }
            List<Sys_Config> configList = _baseSysConfigService.GetModels(a => a.param_name == configLik || a.param_key == configLik).ToList();
            return configList;
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
