using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using Item.ApiServer.BLL.IBLL;
using System.Collections.Generic;

namespace ApiServer.BLL.BLL
{
    public class SysApiService : ISysApiService
    {
        private readonly IBaseService<Sys_Api> _baseService;
        private readonly IMySystemService _mySystemService;

        public SysApiService(IBaseService<Sys_Api> baseService, IMySystemService mySystemService)
        {
            _baseService = baseService;
            _mySystemService = mySystemService;
        }

        public List<SysApiNode> GetApiTreeById(string apiNameLike, bool apiStatus)
        {
            // Sys_Api rootSysApi = _baseService
            return new List<SysApiNode>();
        }
    }
}
