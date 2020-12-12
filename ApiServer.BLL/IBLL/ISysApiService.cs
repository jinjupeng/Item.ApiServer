using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysApiService
    {
        List<SysApiNode> GetApiTreeById(string apiNameLike, bool apiStatus);

        void UpdateApi(Sys_Api sys_Api);

        void AddApi(Sys_Api sys_Api);

        void DeleteApi(Sys_Api sys_Api);

        List<string> GetCheckedKeys(long roleId);

        List<string> GetExpandedKeys();

        void SaveCheckedKeys(long roleId, List<long> checkedIds);

        void UpdateStatus(long id, bool status);

    }
}
