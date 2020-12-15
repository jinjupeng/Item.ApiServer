using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.DAL.IDAL
{
    public interface IMySystemDal
    {
        IQueryable<Sys_Org> SelectOrgTree(long rootOrgId, string orgNameLike, bool orgStatus);

        IQueryable<Sys_Menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool menuStatus);

        IQueryable<Sys_Api> SelectApiTree(long rootApiId, string apiNameLike, bool apiStatus);

        int InsertRoleMenuIds(long roleId, List<long> checkedIds);

        int InsertRoleApiIds(long roleId, List<long> checkedIds);

        IQueryable<string> SelectApiExpandedKeys();

        IQueryable<string> SelectMenuExpandedKeys();

        IQueryable<string> SelectApiCheckedKeys(long roleId);

        IQueryable<string> SelectMenuCheckedKeys(long roleId);

        IQueryable<string> GetCheckedRoleIds(long userId);

        long InsertUserRoleIds(long userId, List<long> checkedIds);

        IQueryable<Sys_Menu> SelectMenuByUserName(string userName);

        IQueryable<SysUserOrg> SelectUser(long? orgId,
                                      string userName,
                                      string phone,
                                      string email,
                                      bool? enabled,
                                      DateTime? createStartTime,
                                      DateTime? createEndTime);

    }
}
