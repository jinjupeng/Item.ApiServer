using ApiServer.BLL.IBLL;
using ApiServer.Common;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using ApiServer.Model.Model.ViewModel;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApiServer.BLL.BLL
{
    public class SysRoleService : ISysRoleService
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
        public MsgModel QueryRoles(string roleLik)
        {
            MsgModel msg = new MsgModel
            {
                message = "查询成功！",
                isok = true
            };
            Expression<Func<Sys_Role, bool>> express = null;
            if (!string.IsNullOrWhiteSpace(roleLik))
            {
                express = a => a.role_code.Contains(roleLik) || a.role_desc.Contains(roleLik) || a.role_name.Contains(roleLik);
            }
            TypeAdapterConfig<Sys_Role, SysRole>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            var sysRoleList = _baseSysRoleService.GetModels(express).ToList();
            msg.data = sysRoleList.BuildAdapter().AdaptToType<List<SysRole>>();
            return msg;

        }

        public MsgModel UpdateRole(Sys_Role sys_Role)
        {
            MsgModel msg = new MsgModel
            {
                message = "更新角色成功！"
            };
            if (!_baseSysRoleService.UpdateRange(sys_Role))
            {
                msg.isok = false;
                msg.message = "更新角色失败！";
            }
            return msg;
        }

        public MsgModel AddRole(Sys_Role sys_Role)
        {
            MsgModel msg = new MsgModel
            {
                message = "新增角色成功！"
            };
            sys_Role.id = new Snowflake().GetId();
            sys_Role.status = false;// 是否禁用:false
            if (!_baseSysRoleService.AddRange(sys_Role))
            {
                msg.isok = false;
                msg.message = "新增角色失败！";
            }
            return msg;
        }

        public MsgModel DeleteRole(long id)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除角色成功！"
            };
            if (!_baseSysRoleService.DeleteRange(_baseSysRoleService.GetModels(a => a.id == id)))
            {
                msg.isok = false;
                msg.message = "删除角色失败！";
            }
            return msg;
        }

        /// <summary>
        /// 获取角色记录及某用户勾选角色记录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgModel GetRolesAndChecked(long userId)
        {
            MsgModel msg = new MsgModel
            {
                message = "查询成功！",
                isok = true
            };
            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                // 所有角色记录
                { "roleDatas", _baseSysRoleService.GetModels(null).ToList() },
                //某用户具有的角色id列表
                { "checkedRoleIds", _mySystemService.GetCheckedRoleIds(userId) }
            };
            msg.data = dict;
            return msg;
        }

        /// <summary>
        /// 保存某用户勾选的角色id数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="checkedIds"></param>
        public MsgModel SaveCheckedKeys(long userId, List<long> checkedIds)
        {
            MsgModel msg = new MsgModel
            {
                message = "用户角色保存成功！"
            };
            _sysUserRoleService.DeleteRange(_sysUserRoleService.GetModels(a => a.user_id == userId).ToList());
            _mySystemService.InsertUserRoleIds(userId, checkedIds);
            return msg;
        }

        /// <summary>
        /// 角色管理：更新角色的禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public MsgModel UpdateStatus(long id, bool status)
        {
            MsgModel msg = new MsgModel
            {
                message = "角色禁用状态更新成功！"
            };
            Sys_Role sys_Role = new Sys_Role
            {
                id = id,
                status = status
            };
            if (!_baseSysRoleService.AddRange(sys_Role))
            {
                msg.isok = false;
                msg.message = "角色禁用状态更新失败！";
            }
            return msg;
        }

    }
}
