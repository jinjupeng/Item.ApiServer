using ApiServer.BLL.IBLL;
using ApiServer.Common;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public class SysUserService : ISysUserService
    {
        private readonly IBaseService<Sys_User> _baseSysUserService;
        private readonly IMySystemService _mySystemService;
        private readonly ISysConfigService _sysConfigService;

        public SysUserService(IBaseService<Sys_User> baseSysUserService,
            IMySystemService mySystemService, ISysConfigService sysConfigService)
        {
            _baseSysUserService = baseSysUserService;
            _mySystemService = mySystemService;
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 根据登录用户名查询用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Sys_User GetUserByUserName(string userName)
        {
            Sys_User sys_User = _baseSysUserService.GetModels(a => a.username == userName).SingleOrDefault();
            if (sys_User != null)
            {
                sys_User.password = "";
            }
            return sys_User;
        }

        // TODO：查询


        /// <summary>
        /// 用户管理：修改
        /// </summary>
        /// <param name="sys_User"></param>
        public void UpdateUser(Sys_User sys_User)
        {
            _baseSysUserService.UpdateRange(sys_User);
        }

        /// <summary>
        /// 用户管理：新增
        /// </summary>
        /// <param name="sys_User"></param>
        public void AddUser(Sys_User sys_User)
        {
            sys_User.password = PasswordEncoder.Encode(_sysConfigService.GetConfig("user.init.password").param_value);
            sys_User.create_time = DateTime.Now; //创建时间
            sys_User.enabled = true;//新增用户激活
            _baseSysUserService.AddRange(sys_User);
        }

        /// <summary>
        /// 用户管理：删除
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteUser(long userId)
        {
            _baseSysUserService.DeleteRange(_baseSysUserService.GetModels(a => a.id == userId));
        }

        /// <summary>
        /// 用户管理：重置密码
        /// </summary>
        /// <param name="userId"></param>
        public MsgModel PwdReset(long userId)
        {
            MsgModel msg = new MsgModel
            {
                message = "密码重置成功！",
                isok = true
            };
            Sys_User sys_User = _baseSysUserService.GetModels(a => a.id == userId).ToList().SingleOrDefault();
            sys_User.id = userId;
            sys_User.password = PasswordEncoder.Encode(_sysConfigService.GetConfig("user.init.password").param_value);
            var length = sys_User.password.Length;
            bool result = _baseSysUserService.UpdateRange(sys_User);
            if (!result)
            {
                msg.message = "密码重置失败！";
                msg.isok = false;
            }
            return msg;
        }

        /// <summary>
        /// 判断当前登录的用户密码是否是默认密码，如果是，会让他去修改
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool IsDefault(string userName)
        {
            Sys_User sys_User = _baseSysUserService.GetModels(a => a.username == userName).SingleOrDefault();

            //判断数据库密码是否是默认密码
            return PasswordEncoder.IsMatch(sys_User.password, _sysConfigService.GetConfig("user.init.password").param_value);
        }

        /// <summary>
        /// 个人中心：修改密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="oldPass"></param>
        /// <param name="newPass"></param>
        public MsgModel ChangePwd(string userName, string oldPass, string newPass)
        {
            MsgModel msg = new MsgModel
            {
                message = "密码修改成功！",
                isok = true
            };
            Sys_User sys_User = _baseSysUserService.GetModels(a => a.username == userName).SingleOrDefault();
            // 判断旧密码是否正确
            bool isMatch = PasswordEncoder.IsMatch(sys_User.password, oldPass);
            if (!isMatch)
            {
                msg.message = "原密码输入错误，请确认后重新输入！";
                msg.isok = false;
                return msg;
            }
            sys_User.password = PasswordEncoder.Encode(newPass);
            _baseSysUserService.UpdateRange(sys_User);
            return msg;
        }

        /// <summary>
        /// 用户管理：更新用户的激活状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        public void UpdateEnabled(long id, bool enabled)
        {
            Sys_User sys_User = new Sys_User
            {
                id = id,
                enabled = enabled
            };
            _baseSysUserService.UpdateRange(sys_User);
        }
    }
}
