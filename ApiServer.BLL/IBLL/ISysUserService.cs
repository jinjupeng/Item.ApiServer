using ApiServer.Model.Entity;

namespace ApiServer.BLL.IBLL
{
    public interface ISysUserService
    {
        Sys_User GetUserByUserName(string userName);
        void UpdateUser(Sys_User sys_User);
        void AddUser(Sys_User sys_User);
        void DeleteUser(long userId);
        void UpdateEnabled(long id, bool enabled);
    }
}
