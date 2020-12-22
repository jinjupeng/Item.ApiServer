using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysApiService
    {
        MsgModel GetApiTreeById(string apiNameLike, bool apiStatus);

        MsgModel UpdateApi(Sys_Api sys_Api);

        MsgModel AddApi(Sys_Api sys_Api);

        MsgModel DeleteApi(Sys_Api sys_Api);

        List<string> GetCheckedKeys(long roleId);

        List<string> GetExpandedKeys();

        MsgModel SaveCheckedKeys(long roleId, List<long> checkedIds);

        MsgModel UpdateStatus(long id, bool status);

    }
}
