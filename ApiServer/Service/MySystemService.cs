using ApiServer.DAL.DAL;
using ApiServer.Models.Entity;
using ApiServer.Models.Model.MsgModel;
using ApiServer.Models.Model.PageModel;
using ApiServer.Models.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public interface IMySystemService
    {
        List<sys_org> SelectOrgTree(long rootOrgId, string orgNameLike, bool? orgStatus);

        List<sys_menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool? menuStatus);

        List<sys_api> SelectApiTree(long rootApiId, string apiNameLike, bool apiStatus);

        int InsertRoleMenuIds(long roleId, List<long> checkedIds);

        int InsertRoleApiIds(long roleId, List<long> checkedIds);

        List<string> SelectApiExpandedKeys();

        List<string> SelectMenuExpandedKeys();

        List<string> SelectApiCheckedKeys(long roleId);

        List<string> SelectMenuCheckedKeys(long roleId);

        List<string> GetCheckedRoleIds(long userId);

        long InsertUserRoleIds(long userId, List<long> checkedIds);

        List<sys_menu> SelectMenuByUserName(string userName);
        MsgModel SelectUser(int pageIndex, int pageSize, long? orgId, string userName, string phone, string email, bool? enabled, DateTime? createStartTime, DateTime? createEndTime);
    }

    public class MySystemService : IMySystemService
    {
        private readonly IMySystemDal _mySystemDal;

        public MySystemService(IMySystemDal mySystemDal)
        {
            _mySystemDal = mySystemDal;
        }

        public List<string> GetCheckedRoleIds(long userId)
        {
            return _mySystemDal.GetCheckedRoleIds(userId).ToList();
        }

        public int InsertRoleApiIds(long roleId, List<long> checkedIds)
        {
            return _mySystemDal.InsertRoleApiIds(roleId, checkedIds);
        }

        public int InsertRoleMenuIds(long roleId, List<long> checkedIds)
        {
            return _mySystemDal.InsertRoleMenuIds(roleId, checkedIds);
        }

        public long InsertUserRoleIds(long userId, List<long> checkedIds)
        {
            return _mySystemDal.InsertUserRoleIds(userId, checkedIds);
        }

        public List<string> SelectApiCheckedKeys(long roleId)
        {
            return _mySystemDal.SelectApiCheckedKeys(roleId).ToList();
        }

        public List<string> SelectApiExpandedKeys()
        {
            return _mySystemDal.SelectApiExpandedKeys().ToList();
        }

        public List<sys_api> SelectApiTree(long rootApiId, string apiNameLike, bool apiStatus)
        {
            return _mySystemDal.SelectApiTree(rootApiId, apiNameLike, apiStatus).ToList();
        }

        public List<sys_menu> SelectMenuByUserName(string userName)
        {
            return _mySystemDal.SelectMenuByUserName(userName).ToList();
        }

        public List<string> SelectMenuCheckedKeys(long roleId)
        {
            return _mySystemDal.SelectMenuCheckedKeys(roleId).ToList();
        }

        public List<string> SelectMenuExpandedKeys()
        {
            return _mySystemDal.SelectMenuExpandedKeys().ToList();
        }

        public List<sys_menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool? menuStatus)
        {
            return _mySystemDal.SelectMenuTree(rootMenuId, menuNameLike, menuStatus).ToList();
        }

        public List<sys_org> SelectOrgTree(long rootOrgId, string orgNameLike, bool? orgStatus)
        {
            return _mySystemDal.SelectOrgTree(rootOrgId, orgNameLike, orgStatus).ToList();
        }

        public MsgModel SelectUser(int pageIndex, int pageSize, long? orgId, string userName, string phone, string email, bool? enabled, DateTime? createStartTime, DateTime? createEndTime)
        {
            var result = _mySystemDal.SelectUser(orgId, userName, phone, email, enabled, createStartTime, createEndTime);
            int items = result.Count();
            PageModel<SysUserOrg> pageModel = new PageModel<SysUserOrg>
            {
                pageNum = pageIndex,
                size = pageSize,
                records = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                total = items,
                pageSize = items % pageSize > 0 ? items / pageSize + 1 : items / pageSize // 分页
            };
            return MsgModel.Success(pageModel);
        }
    }
}
