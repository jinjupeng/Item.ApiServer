using ApiServer.BLL.IBLL;
using ApiServer.DAL.IDAL;
using ApiServer.Model.Entity;
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

        public List<Sys_Menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool menuStatus)
        {
            return _mySystemDal.SelectMenuTree(rootMenuId, menuNameLike, menuStatus).ToList();
        }

        public List<Sys_Org> SelectOrgTree(long rootOrgId, string orgNameLike, bool orgStatus)
        {
            return _mySystemDal.SelectOrgTree(rootOrgId, orgNameLike, orgStatus).ToList();
        }
    }
}
