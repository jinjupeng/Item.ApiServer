using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;

namespace ApiServer.BLL.IBLL
{
    public interface ISysUserService
    {
        Sys_User GetUserByUserName(string userName);
        void UpdateUser(Sys_User sys_User);
        void AddUser(Sys_User sys_User);
        void DeleteUser(long userId);
        MsgModel PwdReset(long userId);
        bool IsDefault(string userName);
        MsgModel ChangePwd(string userName, string oldPass, string newPass);
        void UpdateEnabled(long id, bool enabled);
    }
}
