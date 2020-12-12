using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysOrgService
    {
        List<SysOrgNode> GetOrgTreeById(long rootOrgId, string orgNameLike, bool orgStatus);

        void UpdateOrg(Sys_Org sys_Org);

        void AddOrg(Sys_Org sys_Org);

        void DeleteOrg(Sys_Org sys_Org);

        void UpdateStatus(long id, bool status);
    }

}
