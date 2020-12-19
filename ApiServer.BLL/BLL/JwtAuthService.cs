using ApiServer.BLL.IBLL;
using ApiServer.Common;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public class JwtAuthService : IJwtAuthService
    {
        private readonly IBaseService<Sys_User> _baseService;

        public JwtAuthService(IBaseService<Sys_User> baseService)
        {
            _baseService = baseService;
        }

        public MsgModel Login(string username, string password)
        {
            MsgModel msg = new MsgModel()
            {
                isok = true,
                message = "登录成功！"
            };
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                msg.isok = false;
                msg.message = "用户名或密码为空！";
                return msg;
            }
            // 加密登陆密码
            string encodePassword = PasswordEncoder.Encode(password);

            Sys_User sys_User = _baseService.GetModels(a => a.username == username && a.password == encodePassword).SingleOrDefault();
            if (sys_User == null)
            {
                msg.isok = false;
                msg.message = "用户名或密码不正确！";
            }
            else if (sys_User.enabled == false)
            {
                msg.isok = false;
                msg.message = "账户已被禁用！";
            }

            return msg;
        }

    }
}
