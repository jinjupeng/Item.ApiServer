using ApiServer.Model.Entity;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysDictService
    {
        List<Sys_Dict> All();
        List<Sys_Dict> Query(string groupName, string groupCode);

        void Update(Sys_Dict sys_Dict);
        void Add(Sys_Dict sys_Dict);
        void Delete(long id);
    }
}
