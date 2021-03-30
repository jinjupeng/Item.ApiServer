using ApiServer.BLL.IBLL;
using ApiServer.Common;
using ApiServer.Model.Entity;
using ApiServer.Model.Enum;
using ApiServer.Model.Model.MsgModel;
using ApiServer.Model.Model.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Http;
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
        private readonly IBaseService<Sys_User> _sysUserService;

        public SysRoleService(IBaseService<Sys_Role> baseSysRoleService,
            IMySystemService mySystemService, IBaseService<Sys_User_Role> sysUserRoleService,
            IBaseService<Sys_User> sysUserService)
        {
            _baseSysRoleService = baseSysRoleService;
            _mySystemService = mySystemService;
            _sysUserRoleService = sysUserRoleService;
            _sysUserService = sysUserService;
        }

        /// <summary>
        /// 根据用户名获取用户角色（目前只支持单用户单角色）
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetRoleByUserName(string userName)
        {
            string roleCode = string.Empty;
            try
            {
                Sys_User sys_User = _sysUserService.GetModels(a => a.username == userName).SingleOrDefault();
                Sys_User_Role sys_User_Role = _sysUserRoleService.GetModels(a => a.user_id == sys_User.id).SingleOrDefault();
                Sys_Role sys_Role = _baseSysRoleService.GetModels(a => a.id == sys_User_Role.role_id && a.status == false).SingleOrDefault();
                roleCode = sys_Role.role_code;
            }
            catch (Exception ex)
            {
                throw new CustomException(500, "用户角色不存在或角色已被禁用");
            }

            return roleCode;
        }


        /// <summary>
        /// 根据参数查询角色记录
        /// </summary>
        /// <param name="roleLik">角色编码 或角色描述 或角色名称模糊查询</param>
        /// <returns>角色记录列表</returns>
        public MsgModel QueryRoles(string roleLik)
        {
            Expression<Func<Sys_Role, bool>> express = null;
            if (!string.IsNullOrWhiteSpace(roleLik))
            {
                express = a => a.role_code.Contains(roleLik) || a.role_desc.Contains(roleLik) || a.role_name.Contains(roleLik);
            }
            var sysRoleList = _baseSysRoleService.GetModels(express).ToList();
            var data = sysRoleList.BuildAdapter().AdaptToType<List<SysRole>>();
            return MsgModel.Success(data, "查询成功！");
        }

        public MsgModel UpdateRole(Sys_Role sys_Role)
        {
            if (!_baseSysRoleService.UpdateRange(sys_Role))
            {
                return MsgModel.Fail("角色更新失败！");
            }
            return MsgModel.Success("角色更新成功！");
        }

        public MsgModel AddRole(Sys_Role sys_Role)
        {
            CustomException customException = new CustomException();
            sys_Role.id = new Snowflake().GetId();
            sys_Role.status = false;// 是否禁用:false
            if (_baseSysRoleService.GetModels(a => a.role_code == sys_Role.role_code).Any())
            {
                customException.Code = (int)HttpStatusCode.Status500InternalServerError;

                return MsgModel.Fail(StatusCodes.Status500InternalServerError, "角色编码已存在，不能重复！");
            }
            if (!_baseSysRoleService.AddRange(sys_Role))
            {
                return MsgModel.Fail("新增角色失败！");
            }
            return MsgModel.Success("新增角色成功！");
        }

        public MsgModel DeleteRole(long id)
        {
            if (!_baseSysRoleService.DeleteRange(_baseSysRoleService.GetModels(a => a.id == id)))
            {
                return MsgModel.Fail("删除角色失败！");
            }
            return MsgModel.Success("删除角色成功！");
        }

        /// <summary>
        /// 获取角色记录及某用户勾选角色记录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MsgModel GetRolesAndChecked(long userId)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                // 所有角色记录
                { "roleDatas", _baseSysRoleService.GetModels(a => a.status == false).ToList().BuildAdapter().AdaptToType<List<SysRole>>() },
                //某用户具有的角色id列表
                { "checkedRoleIds", _mySystemService.GetCheckedRoleIds(userId) }
            };
            return MsgModel.Success(dict, "查询成功！");
        }

        /// <summary>
        /// 保存某用户勾选的角色id数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="checkedIds"></param>
        public MsgModel SaveCheckedKeys(long userId, List<long> checkedIds)
        {
            _sysUserRoleService.DeleteRange(_sysUserRoleService.GetModels(a => a.user_id == userId).ToList());
            _mySystemService.InsertUserRoleIds(userId, checkedIds);
            return MsgModel.Success("用户角色保存成功！"); ;
        }

        /// <summary>
        /// 角色管理：更新角色的禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public MsgModel UpdateStatus(long id, bool status)
        {
            Sys_Role sys_Role = _baseSysRoleService.GetModels(a => a.id == id).SingleOrDefault();
            sys_Role.status = status;
            bool result = _baseSysRoleService.UpdateRange(sys_Role);

            return result ? MsgModel.Success("角色禁用状态更新成功！") : MsgModel.Fail("角色禁用状态更新失败！");
        }

    }
}
