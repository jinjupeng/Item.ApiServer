using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using ApiServer.Model.Model.MsgModel;
using System.Collections.Generic;

namespace ApiServer.BLL.IBLL
{
    public interface ISysMenuService
    {
        MsgModel GetMenuTree(string menuNameLike, bool? menuStatus);
        void UpdateMenu(Sys_Menu sys_Menu);
        void AddMenu(Sys_Menu sys_Menu);
        void DeleteMenu(Sys_Menu sys_Menu);
        List<string> GetCheckedKeys(long roleId);
        List<string> GetExpandedKeys();
        void SaveCheckedKeys(long roleId, List<long> checkedIds);
        MsgModel GetMenuTreeByUsername(string username);
        void UpdateStatus(long id, bool status);
    }
}
