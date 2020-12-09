using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using Item.ApiServer.BLL.IBLL;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.BLL.BLL
{
    public class SysOrgService : ISysOrgService
    {
        private readonly IBaseService<Sys_Org> _baseSysOrgService;
        private readonly IMySystemService _mySystemService;

        public SysOrgService(IBaseService<Sys_Org> baseSysOrgService, IMySystemService mySystemService)
        {
            _baseSysOrgService = baseSysOrgService;
            _mySystemService = mySystemService;
        }

        /// <summary>
        /// 根据当前登录用户所属组织，查询组织树
        /// </summary>
        /// <param name="rootOrgId">当前登录用户的组织id</param>
        /// <param name="orgNameLike">组织名称参数</param>
        /// <param name="orgStatus">组织状态参数</param>
        /// <returns>组织列表</returns>
        public List<SysOrgNode> GetOrgTreeById(long rootOrgId, string orgNameLike, bool orgStatus)
        {

            return new List<SysOrgNode>();
        }
    }
}
