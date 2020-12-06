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

        public SysApiService(IBaseService<Sys_Api> baseService)
        {
            _baseService = baseService;
        }

        public List<SysApiNode> GetApiTreeById(string apiNameLike, bool apiStatus)
        {
            return new List<SysApiNode>();
        }
    }
}
