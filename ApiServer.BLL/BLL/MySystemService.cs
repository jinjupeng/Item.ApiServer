using ApiServer.BLL.IBLL;
using ApiServer.DAL.IDAL;
using ApiServer.Model.Entity;
using Item.ApiServer.DAL.IDAL;
using System.Collections.Generic;

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
            throw new System.NotImplementedException();
        }

        public int InsertRoleApiIds(long roleId, List<long> checkedIds)
        {
            throw new System.NotImplementedException();
        }

        public int InsertRoleMenuIds(long roleId, List<long> checkedIds)
        {
            throw new System.NotImplementedException();
        }

        public long InsertUserRoleIds(long userId, List<long> checkedIds)
        {
            throw new System.NotImplementedException();
        }

        public List<string> SelectApiCheckedKeys(long roleId)
        {
            throw new System.NotImplementedException();
        }

        public List<string> SelectApiExpandedKeys()
        {
            throw new System.NotImplementedException();
        }

        public List<Sys_Api> SelectApiTree(long rootApiId, string apiNameLike, bool apiStatus)
        {
            throw new System.NotImplementedException();
        }

        public List<Sys_Menu> SelectMenuByUserName(string userName)
        {
            throw new System.NotImplementedException();
        }

        public List<string> SelectMenuCheckedKeys(long roleId)
        {
            throw new System.NotImplementedException();
        }

        public List<string> SelectMenuExpandedKeys()
        {
            throw new System.NotImplementedException();
        }

        public List<Sys_Menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool menuStatus)
        {
            throw new System.NotImplementedException();
        }

        public List<Sys_Org> SelectOrgTree(long rootOrgId, string orgNameLike, bool orgStatus)
        {
            throw new System.NotImplementedException();
        }
    }
}
