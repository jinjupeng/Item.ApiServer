using ApiServer.BLL.IBLL;
using ApiServer.DAL.IDAL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using ApiServer.Model.Model.MsgModel;
using ApiServer.Model.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.BLL.BLL
{
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

        public List<Sys_Api> SelectApiTree(long rootApiId, string apiNameLike, bool apiStatus)
        {
            return _mySystemDal.SelectApiTree(rootApiId, apiNameLike, apiStatus).ToList();
        }

        public List<Sys_Menu> SelectMenuByUserName(string userName)
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

        public List<Sys_Menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool? menuStatus)
        {
            return _mySystemDal.SelectMenuTree(rootMenuId, menuNameLike, menuStatus).ToList();
        }

        public List<Sys_Org> SelectOrgTree(long rootOrgId, string orgNameLike, bool? orgStatus)
        {
            return _mySystemDal.SelectOrgTree(rootOrgId, orgNameLike, orgStatus).ToList();
        }

        public MsgModel SelectUser(int pageIndex, int pageSize, long? orgId, string userName, string phone, string email, bool? enabled, DateTime? createStartTime, DateTime? createEndTime)
        {
            MsgModel msg = new MsgModel()
            {
                isok = true,
                message = "查询成功！"
            };
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
            msg.data = pageModel;
            return msg;
        }
    }
}
