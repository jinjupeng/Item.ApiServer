using ApiServer.Model.Entity;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysRoleService
    {
        List<Sys_Role> QueryRoles(string roleLik);

        void UpdateRole(Sys_Role sys_Role);
        void AddRole(Sys_Role sys_Role);
        void DeleteRole(long id);
        Dictionary<string, object> GetRolesAndChecked(long userId);
        void SaveCheckedKeys(long userId, List<long> checkedIds);
        void UpdateStatus(long id, bool status);
    }
}
