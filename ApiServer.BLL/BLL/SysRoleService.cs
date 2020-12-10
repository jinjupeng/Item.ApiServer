using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public class SysRoleService : BaseService<Sys_Role>, ISysRoleService
    {
        private readonly IBaseService<Sys_Role> _baseSysRoleService;
        private readonly IMySystemService _mySystemService;
        private readonly IBaseService<Sys_User_Role> _sysUserRoleService;

        public SysRoleService(IBaseService<Sys_Role> baseSysRoleService,
            IMySystemService mySystemService, IBaseService<Sys_User_Role> sysUserRoleService)
        {
            _baseSysRoleService = baseSysRoleService;
            _mySystemService = mySystemService;
            _sysUserRoleService = sysUserRoleService;
        }

        /// <summary>
        /// 根据参数查询角色记录
        /// </summary>
        /// <param name="roleLik">角色编码 或角色描述 或角色名称模糊查询</param>
        /// <returns>角色记录列表</returns>
        public List<Sys_Role> QueryRoles(string roleLik)
        {
            return _baseSysRoleService.GetModels(a => a.role_code.Contains(roleLik) || a.role_desc.Contains(roleLik) || a.role_name.Contains(roleLik)).ToList();

        }

        public void UpdateRole(Sys_Role sys_Role)
        {
            _baseSysRoleService.UpdateRange(sys_Role);
        }

        public void AddRole(Sys_Role sys_Role)
        {
            sys_Role.status = false;// 是否禁用:false
            _baseSysRoleService.AddRange(sys_Role);
        }

        public void DeleteRole(long id)
        {
            _baseSysRoleService.DeleteRange(_baseSysRoleService.GetModels(a => a.id == id));
        }

        /// <summary>
        /// 获取角色记录及某用户勾选角色记录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetRolesAndChecked(long userId)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            // 所有角色记录
            ret.Add("roleDatas", _baseSysRoleService.GetModels(null).ToList());
            //某用户具有的角色id列表
            ret.Add("checkedRoleIds", _mySystemService.GetCheckedRoleIds(userId));

            return ret;
        }

        /// <summary>
        /// 保存某用户勾选的角色id数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="checkedIds"></param>
        public void SaveCheckedKeys(long userId, List<long> checkedIds)
        {
            _sysUserRoleService.DeleteRange(_sysUserRoleService.GetModels(a => a.user_id == userId).ToList());
            _mySystemService.InsertUserRoleIds(userId, checkedIds);
        }

        /// <summary>
        /// 角色管理：更新角色的禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public void UpdateStatus(long id, bool status)
        {
            Sys_Role sys_Role = new Sys_Role
            {
                id = id,
                status = status
            };
            _baseSysRoleService.AddRange(sys_Role);
        }

    }
}
