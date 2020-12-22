using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysDictService
    {
        MsgModel All();
        MsgModel Query(string groupName, string groupCode);

        MsgModel Update(Sys_Dict sys_Dict);
        MsgModel Add(Sys_Dict sys_Dict);
        MsgModel Delete(long id);
    }
}
