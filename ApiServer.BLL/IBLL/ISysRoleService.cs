using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysRoleService
    {
        MsgModel QueryRoles(string roleLik);

        void UpdateRole(Sys_Role sys_Role);
        void AddRole(Sys_Role sys_Role);
        void DeleteRole(long id);
        Dictionary<string, object> GetRolesAndChecked(long userId);
        void SaveCheckedKeys(long userId, List<long> checkedIds);
        void UpdateStatus(long id, bool status);
    }
}
