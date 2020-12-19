using ApiServer.Model.Model.MsgModel;

namespace ApiServer.BLL.IBLL
{
    public interface IJwtAuthService
    {
        MsgModel Login(string username, string password);
    }
}
