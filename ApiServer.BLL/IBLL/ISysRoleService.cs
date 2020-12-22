using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysRoleService
    {
        MsgModel QueryRoles(string roleLik);

        MsgModel UpdateRole(Sys_Role sys_Role);
        MsgModel AddRole(Sys_Role sys_Role);
        MsgModel DeleteRole(long id);
        MsgModel GetRolesAndChecked(long userId);
        MsgModel SaveCheckedKeys(long userId, List<long> checkedIds);
        MsgModel UpdateStatus(long id, bool status);
    }
}
