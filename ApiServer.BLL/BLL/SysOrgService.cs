using ApiServer.BLL.IBLL;
using ApiServer.Model;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using Item.ApiServer.BLL.BLL;
using Item.ApiServer.BLL.IBLL;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public class SysOrgService : BaseService<Sys_Org>, ISysOrgService
    {
        private readonly IBaseService<Sys_Org> _baseSysOrgService;
        private readonly IMySystemService _mySystemService;

        public SysOrgService(IBaseService<Sys_Org> baseSysOrgService, IMySystemService mySystemService)
        {
            _baseSysOrgService = baseSysOrgService;
            _mySystemService = mySystemService;
        }

        /// <summary>
        /// 根据当前登录用户所属组织，查询组织树
        /// </summary>
        /// <param name="rootOrgId">当前登录用户的组织id</param>
        /// <param name="orgNameLike">组织名称参数</param>
        /// <param name="orgStatus">组织状态参数</param>
        /// <returns>组织列表</returns>
        public List<SysOrgNode> GetOrgTreeById(long rootOrgId, string orgNameLike, bool orgStatus)
        {
            List<Sys_Org> sysOrgs = _mySystemService.SelectOrgTree(rootOrgId, orgNameLike, orgStatus);
            List<SysOrgNode> sysOrgNodes = new List<SysOrgNode>();
            foreach (Sys_Org sys_Org in sysOrgs)
            {
                SysOrgNode sysOrgNode = new SysOrgNode
                {
                    id = sys_Org.id,
                    org_pid = sys_Org.org_pid,
                    org_pids = sys_Org.org_pids,
                    is_leaf = sys_Org.is_leaf,
                    org_name = sys_Org.org_name,
                    address = sys_Org.address,
                    phone = sys_Org.phone,
                    email = sys_Org.email,
                    sort = sys_Org.sort,
                    level = sys_Org.level,
                    status = sys_Org.status,
                };
                sysOrgNodes.Add(sysOrgNode);
            }
            if (!string.IsNullOrEmpty(orgNameLike))
            {
                //根据组织名称查询，返回平面列表
                return sysOrgNodes;
            }
            else
            {
                //否则返回树型结构列表
                return DataTreeUtil<SysOrgNode, long>.BuildTree(sysOrgNodes, rootOrgId);
            }

        }

        public void UpdateOrg(Sys_Org sys_Org)
        {
            _baseSysOrgService.UpdateRange(sys_Org);
        }

        public void AddOrg(Sys_Org sys_Org)
        {
            SetOrgIdsAndLevel(sys_Org);
            sys_Org.is_leaf = true;//新增的组织节点都是子节点，没有下级
            Sys_Org parent = new Sys_Org();
            parent.id = sys_Org.org_pid;
            parent.is_leaf = false; //更新父节点为非子节点。
            _baseSysOrgService.UpdateRange(parent);

            sys_Org.status = false;//设置是否禁用，新增节点默认可用
            _baseSysOrgService.AddRange(sys_Org);
        }

        public void DeleteOrg(Sys_Org sys_Org)
        {
            List<Sys_Org> myChilds = _baseSysOrgService.GetModels(a => a.org_pids.Contains("[" + sys_Org.org_pid + "]")).ToList();
            if (myChilds.Count > 0)
            {
                // "不能删除有下级组织的组织机构"
            }


            List<Sys_Org> myFatherChilds = _baseSysOrgService.GetModels(a => a.org_pids.Contains("[" + "]")).ToList();
            //我的父节点只有我这一个子节点，而我还要被删除，更新父节点为叶子节点。
            if (myFatherChilds.Count == 1)
            {
                Sys_Org parent = new Sys_Org();
                parent.id = sys_Org.org_pid;
                parent.is_leaf = true;// 更新父节点为叶子节点。
                _baseSysOrgService.UpdateRange(parent);
            }
            // 删除节点
            _baseSysOrgService.DeleteRange(sys_Org);
        }

        /// <summary>
        /// //设置某子节点的所有祖辈id
        /// </summary>
        /// <param name="child"></param>
        private void SetOrgIdsAndLevel(Sys_Org child)
        {
            List<Sys_Org> allOrgs = _baseSysOrgService.GetModels(null).ToList();
            foreach (Sys_Org sys_Org in allOrgs)
            {
                //从组织列表中找到自己的直接父亲
                if (sys_Org.id == child.org_pid)
                {
                    //直接父亲的所有祖辈id + 直接父id = 当前子节点的所有祖辈id
                    //爸爸的所有祖辈 + 爸爸 = 孩子的所有祖辈
                    child.org_pids = sys_Org.org_pids + "[" + child.org_pid + "]";
                    child.level = sys_Org.level + 1;
                }
            }
        }

        /// <summary>
        /// 组织管理：更新组织的禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public void UpdateStatus(long id, bool status)
        {
            Sys_Org sys_Org = new Sys_Org
            {
                id = id,
                status = status
            };
            _baseSysOrgService.UpdateRange(sys_Org);
        }
    }
}
