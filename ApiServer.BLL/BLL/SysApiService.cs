using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using Item.ApiServer.BLL.IBLL;
using System.Collections.Generic;

namespace ApiServer.BLL.BLL
{
    public class SysApiService : ISysApiService
    {
        private readonly IBaseService<Sys_Api> _baseService;
        private readonly IMySystemService _mySystemService;

        public SysApiService(IBaseService<Sys_Api> baseService, IMySystemService mySystemService)
        {
            _baseService = baseService;
            _mySystemService = mySystemService;
        }

        public List<SysApiNode> GetApiTreeById(string apiNameLike, bool apiStatus)
        {
            // Sys_Api rootSysApi = _baseService
            return new List<SysApiNode>();
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

        /// <summary>
        /// 设置某子节点的所有祖辈id
        /// </summary>
        /// <param name="child"></param>
        private void SetApiIdsAndLevel(Sys_Api child)
        {
            List<Sys_Api> allApis = (List<Sys_Api>)_baseService.GetModels(null);
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
    }
}
