using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using Item.ApiServer.BLL.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiServer.BLL.BLL
{
    public class SysMenuService : ISysMenuService
    {
        private readonly IBaseService<Sys_Menu> _baseSysMenuService;
        private readonly IMySystemService _mySystemService;

        public SysMenuService(IBaseService<Sys_Menu> baseSysMenuService, IMySystemService mySystemService)
        {
            _baseSysMenuService = baseSysMenuService;
            _mySystemService = mySystemService;
        }

        public List<SysMenuNode> GetMenuTree(string menuNameLike, bool menuStatus)
        {
            //保证数据库里面level=1的节点只有一个，根节点
            Sys_Menu rootSysMenu = _baseSysMenuService.GetModels(a => a.level == 1).Single();
            if(rootSysMenu != null)
            {
                long rootMenuId = rootSysMenu.id;
                List<Sys_Menu> sysMenus = _mySystemService.SelectMenuTree(rootMenuId, menuNameLike, menuStatus);


            }
        }
    }
}
