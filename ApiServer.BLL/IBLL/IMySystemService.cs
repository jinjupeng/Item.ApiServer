using ApiServer.Model.Entity;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface IMySystemService
    {
        List<Sys_Org> SelectOrgTree(long rootOrgId, string orgNameLike, bool orgStatus);

        List<Sys_Menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool menuStatus);

        List<Sys_Api> SelectApiTree(long rootApiId, string apiNameLike, bool apiStatus);

        int InsertRoleMenuIds(long roleId, List<long> checkedIds);

        int InsertRoleApiIds(long roleId, List<long> checkedIds);

        List<string> SelectApiExpandedKeys();

        List<string> SelectMenuExpandedKeys();

        List<string> SelectApiCheckedKeys(long roleId);

        List<string> SelectMenuCheckedKeys(long roleId);

        List<string> GetCheckedRoleIds(long userId);

        long InsertUserRoleIds(long userId, List<long> checkedIds);

        List<Sys_Menu> SelectMenuByUserName(string userName);
    }
}
