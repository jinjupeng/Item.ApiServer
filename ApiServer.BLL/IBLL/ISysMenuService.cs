using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysMenuService
    {
        MsgModel GetMenuTree(string menuNameLike, bool? menuStatus);
        MsgModel UpdateMenu(Sys_Menu sys_Menu);
        MsgModel AddMenu(Sys_Menu sys_Menu);
        MsgModel DeleteMenu(Sys_Menu sys_Menu);
        List<string> GetCheckedKeys(long roleId);
        List<string> GetExpandedKeys();
        MsgModel SaveCheckedKeys(long roleId, List<long> checkedIds);
        MsgModel GetMenuTreeByUsername(string username);
        MsgModel UpdateStatus(long id, bool status);
    }
}
