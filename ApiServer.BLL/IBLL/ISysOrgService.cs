using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;

namespace ApiServer.BLL.IBLL
{
    public interface ISysOrgService
    {
        MsgModel GetOrgTreeById(long rootOrgId, string orgNameLike, bool? orgStatus);

        MsgModel UpdateOrg(Sys_Org sys_Org);

        MsgModel AddOrg(Sys_Org sys_Org);

        MsgModel DeleteOrg(Sys_Org sys_Org);

        MsgModel UpdateStatus(long id, bool status);
    }

}
