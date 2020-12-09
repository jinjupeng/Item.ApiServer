using ApiServer.BLL.IBLL;
using ApiServer.Model;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using Item.ApiServer.BLL.IBLL;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public class SysApiService : ISysApiService
    {
        private readonly IBaseService<Sys_Api> _baseService;
        private readonly IBaseService<Sys_Role_Api> _baseSysRoleApiService;
        private readonly IMySystemService _mySystemService;

        public SysApiService(IBaseService<Sys_Api> baseService, IMySystemService mySystemService, IBaseService<Sys_Role_Api> baseSysRoleApiService)
        {
            _baseService = baseService;
            _mySystemService = mySystemService;
            _baseSysRoleApiService = baseSysRoleApiService;
        }

        public List<SysApiNode> GetApiTreeById(string apiNameLike, bool apiStatus)
        {
            //查找level=1的API节点，即：根节点
            Sys_Api rootSysApi = _baseService.GetModels(s => s.level == 1).Single();
            if(rootSysApi != null)
            {
                long rootApiId = rootSysApi.id;
                List<Sys_Api> sysApis = _mySystemService.SelectApiTree(rootApiId, apiNameLike, apiStatus);

                List<SysApiNode> sysApiNodes = new List<SysApiNode>();
                foreach(Sys_Api sys_Api in sysApis)
                {
                    SysApiNode sysApiNode = new SysApiNode
                    {
                        id = sys_Api.id,
                        api_pid = sys_Api.id,
                        api_pids = sys_Api.api_pids,
                        is_leaf = sys_Api.is_leaf,
                        api_name = sys_Api.api_name,
                        url = sys_Api.url,
                        sort = sys_Api.sort,
                        level = sys_Api.level,
                        status = sys_Api.status
                    };
                    sysApiNodes.Add(sysApiNode);
                }

                if (!string.IsNullOrEmpty(apiNameLike))
                {
                    //根据api名称等查询会破坏树形结构，返回平面列表
                    return sysApiNodes;
                }
                else
                {
                    //否则返回树型结构列表
                    return DataTreeUtil<SysApiNode, long>.BuildTree(sysApiNodes, rootApiId);
                }
            }
            return null;
        }

        public void UpdateApi(Sys_Api sys_Api)
        {
            _baseService.UpdateRange(sys_Api);
        }

        public void AddApi(Sys_Api sys_Api)
        {
            SetApiIdsAndLevel(sys_Api);
            sys_Api.is_leaf = true;//新增的菜单节点都是子节点，没有下级
            Sys_Api parent = new Sys_Api();
            parent.id = sys_Api.api_pid;
            parent.is_leaf = false;//更新父节点为非子节点。
            _baseService.UpdateRange(parent);
            sys_Api.status = false;//设置是否禁用，新增节点默认可用
            _baseService.AddRange(sys_Api);
        }

        public void DeleteApi(Sys_Api sys_Api)
        {
            // 查找被删除节点的子节点
            List<Sys_Api> myChild = _baseService.GetModels(s => s.api_pids.Contains("[" + sys_Api.id + "]")).ToList();
            if(myChild.Count > 0)
            {
                // "不能删除含有下级API接口的节点"
            }
            //查找被删除节点的父节点
            List<Sys_Api> myFatherChild = _baseService.GetModels(s => s.api_pids.Contains("[" + sys_Api.api_pid + "]")).ToList();
            //我的父节点只有我这一个子节点，而我还要被删除，更新父节点为叶子节点。
            if(myFatherChild.Count == 1)
            {
                Sys_Api parent = new Sys_Api();
                parent.id = sys_Api.api_pid;
                parent.is_leaf = true; // //更新父节点为叶子节点。
                _baseService.UpdateRange(parent);
            }
            // 删除节点
            _baseService.DeleteRange(sys_Api);

        }

        /// <summary>
        /// 设置某子节点的所有祖辈id
        /// </summary>
        /// <param name="child"></param>
        private void SetApiIdsAndLevel(Sys_Api child)
        {
            List<Sys_Api> allApis = _baseService.GetModels(null).ToList();
            foreach(var sysApi in allApis)
            {
                // 从组织列表中找到自己的直接父亲
                if(sysApi.id == child.api_pid)
                {
                    //直接父亲的所有祖辈id + 直接父id = 当前子节点的所有祖辈id
                    //爸爸的所有祖辈 + 爸爸 = 孩子的所有祖辈
                    child.api_pids = sysApi.api_pids + ",[" + child.api_pid + "]";
                    child.level = sysApi.level + 1;
                }
            }
        }

        /// <summary>
        /// 获取某角色勾选的API访问权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<string> GetCheckedKeys(long roleId)
        {
            return _mySystemService.SelectApiCheckedKeys(roleId);
        }

        /// <summary>
        /// 获取在API分类树中展开的项
        /// </summary>
        /// <returns></returns>
        public List<string> GetExpandedKeys()
        {
            return _mySystemService.SelectApiExpandedKeys();
        }

        /// <summary>
        /// 保存为某角色新勾选的API项
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="checkedIds"></param>
        public void SaveCheckedKeys(long roleId, List<long> checkedIds)
        {
            // 保存之前先删除
            var sysRoleApiList = _baseSysRoleApiService.GetModels(a => a.role_id == roleId);
            _baseSysRoleApiService.DeleteRange(sysRoleApiList);
            _mySystemService.InsertRoleApiIds(roleId, checkedIds);
        }

        /// <summary>
        /// 接口管理：更新接口的禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public void UpdateStatus(long id, bool status)
        {
            Sys_Api sys_Api = new Sys_Api();
            sys_Api.id = id;
            sys_Api.status = status;
            _baseService.UpdateRange(sys_Api);
        }
    }
}
