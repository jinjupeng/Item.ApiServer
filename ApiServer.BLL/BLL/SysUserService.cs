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

        /// <summary>
        /// 用户管理：查询
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <param name="enabled"></param>
        /// <param name="createStartTime"></param>
        /// <param name="createEndTime"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public MsgModel QueryUser(long? orgId, string userName, string phone, string email, bool? enabled, DateTime? createStartTime, DateTime? createEndTime, int pageNum, int pageSize)
        {
            return _mySystemService.SelectUser(pageNum, pageSize, orgId, userName, phone, email, enabled, createStartTime, createEndTime);
        }


        /// <summary>
        /// 用户管理：修改
        /// </summary>
        /// <param name="sys_User"></param>
        public MsgModel UpdateUser(Sys_User sys_User)
        {
            MsgModel msg = new MsgModel
            {
                message = "更新用户成功！"
            };
            if (!_baseSysUserService.UpdateRange(sys_User))
            {
                msg.isok = false;
                msg.message = "更新用户失败！";
            }
            return msg;
        }

        /// <summary>
        /// 用户管理：新增
        /// </summary>
        /// <param name="sys_User"></param>
        public MsgModel AddUser(Sys_User sys_User)
        {
            MsgModel msg = new MsgModel
            {
                message = "新增用户成功！"
            };
            sys_User.id = new Snowflake().GetId();
            sys_User.password = PasswordEncoder.Encode(_sysConfigService.GetConfigItem("user.init.password"));
            sys_User.create_time = DateTime.Now; //创建时间
            sys_User.enabled = true;//新增用户激活
            if (!_baseSysUserService.AddRange(sys_User))
            {
                msg.isok = false;
                msg.message = "新增用户失败！";
            }
            return msg;
        }

        /// <summary>
        /// 用户管理：删除
        /// </summary>
        /// <param name="userId"></param>
        public MsgModel DeleteUser(long userId)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除用户成功！"
            };
            if (!_baseSysUserService.DeleteRange(_baseSysUserService.GetModels(a => a.id == userId)))
            {
                msg.isok = false;
                msg.message = "删除用户失败！";
            }
            return msg;
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
            sys_User.password = PasswordEncoder.Encode(_sysConfigService.GetConfigItem("user.init.password"));
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
        public MsgModel IsDefault(string userName)
        {
            MsgModel msg = new MsgModel
            {
                message = "获取成功！",
                isok = true
            };
            Sys_User sys_User = _baseSysUserService.GetModels(a => a.username == userName).SingleOrDefault();
            //判断数据库密码是否是默认密码
            msg.data = PasswordEncoder.IsMatch(sys_User.password, _sysConfigService.GetConfigItem("user.init.password"));
            //判断数据库密码是否是默认密码
            return msg;
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
        public MsgModel UpdateEnabled(long id, bool enabled)
        {
            MsgModel msg = new MsgModel
            {
                message = "用户状态更新成功！"
            };
            Sys_User sys_User = new Sys_User
            {
                id = id,
                enabled = enabled
            };
            if (!_baseSysUserService.UpdateRange(sys_User))
            {
                msg.isok = false;
                msg.message = "用户状态更新失败！";
            }
            return msg;
        }
    }
}
