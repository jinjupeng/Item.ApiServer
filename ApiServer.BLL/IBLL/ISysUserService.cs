using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System;

namespace ApiServer.BLL.IBLL
{
    public interface ISysUserService
    {
        Sys_User GetUserByUserName(string userName);
        MsgModel UpdateUser(Sys_User sys_User);
        MsgModel AddUser(Sys_User sys_User);
        MsgModel DeleteUser(long userId);
        MsgModel PwdReset(long userId);
        bool IsDefault(string userName);
        MsgModel ChangePwd(string userName, string oldPass, string newPass);
        MsgModel UpdateEnabled(long id, bool enabled);
        MsgModel QueryUser(long? orgId, string userName, string phone, string email, bool? enabled, DateTime? createStartTime, DateTime? createEndTime, int pageNum, int pageSize);
    }
}
