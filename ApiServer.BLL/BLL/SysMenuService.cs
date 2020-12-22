using ApiServer.BLL.IBLL;
using ApiServer.Common;
using ApiServer.Model;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using ApiServer.Model.Model.MsgModel;
using Mapster;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.BLL.BLL
{
    public class SysMenuService : ISysMenuService
    {
        private readonly IBaseService<Sys_Menu> _baseSysMenuService;
        private readonly IMySystemService _mySystemService;
        private readonly IBaseService<Sys_Role_Menu> _baseSysRoleMenuService;

        public SysMenuService(IBaseService<Sys_Menu> baseSysMenuService, IMySystemService mySystemService, IBaseService<Sys_Role_Menu> baseSysRoleMenuService)
        {
            _baseSysMenuService = baseSysMenuService;
            _mySystemService = mySystemService;
            _baseSysRoleMenuService = baseSysRoleMenuService;
        }

        /// <summary>
        /// 菜单管理：根据查询条件查询树形结构菜单列表
        /// </summary>
        /// <param name="menuNameLike">菜单名称</param>
        /// <param name="menuStatus">菜单可用状态</param>
        /// <returns>菜单列表或树形列表</returns>
        public MsgModel GetMenuTree(string menuNameLike, bool? menuStatus)
        {
            MsgModel msg = new MsgModel
            {
                message = "查询成功！",
                isok = true
            };
            TypeAdapterConfig<Sys_Menu, SysMenuNode>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            //保证数据库里面level=1的节点只有一个，根节点
            Sys_Menu rootSysMenu = _baseSysMenuService.GetModels(a => a.level == 1).Single();
            if (rootSysMenu != null)
            {
                long rootMenuId = rootSysMenu.id;
                List<Sys_Menu> sysMenus = _mySystemService.SelectMenuTree(rootMenuId, menuNameLike, menuStatus);

                List<SysMenuNode> sysMenuNodes = new List<SysMenuNode>();
                foreach (Sys_Menu sys_Menu in sysMenus)
                {
                    //SysMenuNode sysMenuNode = new SysMenuNode
                    //{
                    //    id = sys_Menu.id,
                    //    menu_pid = sys_Menu.menu_pid,
                    //    menu_pids = sys_Menu.menu_pids,
                    //    is_leaf = sys_Menu.is_leaf,
                    //    menu_name = sys_Menu.menu_name,
                    //    url = sys_Menu.url,
                    //    icon = sys_Menu.icon,
                    //    sort = sys_Menu.sort,
                    //    level = sys_Menu.level,
                    //    status = sys_Menu.status,
                    //};
                    // SysMenuNode sysMenuNode = sys_Menu.Adapt<SysMenuNode>();
                    SysMenuNode sysMenuNode = sys_Menu.BuildAdapter().AdaptToType<SysMenuNode>();
                    sysMenuNodes.Add(sysMenuNode);
                }
                if (!string.IsNullOrEmpty(menuNameLike))
                {
                    // 根据菜单名称查询，返回平面列表
                    msg.data = sysMenuNodes;
                    return msg;
                }
                else
                {
                    // 否则返回菜单的树型结构列表
                    msg.data = DataTreeUtil<SysMenuNode, long>.BuildTree(sysMenuNodes, rootMenuId);
                    return msg;
                }
            }
            else
            {
                // "请先在数据库内为菜单配置一个分类的根节点，level=1"
                msg.data = new List<SysMenuNode>();
                return msg;
            }
        }

        public MsgModel UpdateMenu(Sys_Menu sys_Menu)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "更新菜单项成功！"
            };
            _baseSysMenuService.UpdateRange(sys_Menu);
            return msg;
        }

        public MsgModel AddMenu(Sys_Menu sys_Menu)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "新增菜单项成功！"
            };
            sys_Menu.id = new Snowflake().GetId();
            SetMenuIdsAndLevel(sys_Menu);
            sys_Menu.is_leaf = true;//新增的菜单节点都是子节点，没有下级
            Sys_Menu parent = new Sys_Menu
            {
                id = sys_Menu.menu_pid,
                is_leaf = false//更新父节点为非子节点。
            };
            _baseSysMenuService.UpdateRange(parent);
            sys_Menu.status = false;//设置是否禁用，新增节点默认可用
            _baseSysMenuService.AddRange(sys_Menu);
            return msg;
        }

        public MsgModel DeleteMenu(Sys_Menu sys_Menu)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "删除菜单项成功！"
            };
            //查找被删除节点的子节点
            List<Sys_Menu> myChilds = _baseSysMenuService.GetModels(a => a.menu_pids.Contains("[" + sys_Menu.id + "]")).ToList();

            if (myChilds.Count > 0)
            {
                // "不能删除含有下级菜单的菜单"
            }
            //查找被删除节点的父节点
            List<Sys_Menu> myFatherChilds = _baseSysMenuService.GetModels(a => a.menu_pids.Contains("[" + sys_Menu.menu_pid + "]")).ToList();

            //我的父节点只有我这一个子节点，而我还要被删除，更新父节点为叶子节点。
            if (myFatherChilds.Count == 1)
            {
                Sys_Menu parent = new Sys_Menu
                {
                    id = sys_Menu.menu_pid,
                    is_leaf = true//更新父节点为叶子节点。
                };
                _baseSysMenuService.UpdateRange(parent);
            }
            // 删除节点
            _baseSysMenuService.DeleteRange(sys_Menu);
            return msg;
        }

        /// <summary>
        /// 设置某子节点的所有祖辈id
        /// </summary>
        /// <param name="child"></param>
        private void SetMenuIdsAndLevel(Sys_Menu child)
        {
            List<Sys_Menu> allMenus = _baseSysMenuService.GetModels(null).ToList();
            foreach (Sys_Menu sys_Menu in allMenus)
            {
                // 从组织列表中找到自己的直接父亲
                if (sys_Menu.id == child.menu_pid)
                {
                    //直接父亲的所有祖辈id + 直接父id = 当前子节点的所有祖辈id
                    //爸爸的所有祖辈 + 爸爸 = 孩子的所有祖辈
                    child.menu_pids = sys_Menu.menu_pids + ",[" + child.menu_pid + "]";
                    child.level = sys_Menu.level + 1;
                }
            }
        }

        /// <summary>
        /// 获取某角色勾选的菜单权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<string> GetCheckedKeys(long roleId)
        {
            return _mySystemService.SelectMenuCheckedKeys(roleId);
        }

        /// <summary>
        /// 获取在菜单中展开的菜单项
        /// </summary>
        /// <returns></returns>
        public List<string> GetExpandedKeys()
        {
            return _mySystemService.SelectMenuExpandedKeys();
        }

        /// <summary>
        /// 保存为某角色新勾选的菜单项目
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="checkedIds"></param>
        public MsgModel SaveCheckedKeys(long roleId, List<long> checkedIds)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "保存菜单权限成功！"
            };
            // 保存之前先删除
            _baseSysRoleMenuService.DeleteRange(_baseSysRoleMenuService.GetModels(a => a.role_id == roleId).ToList());
            _mySystemService.InsertRoleMenuIds(roleId, checkedIds);
            return msg;
        }

        /// <summary>
        /// 根据某用户的用户名查询该用户可以访问的菜单项（系统左侧边栏菜单）
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public MsgModel GetMenuTreeByUsername(string username)
        {
            MsgModel msg = new MsgModel()
            {
                isok = true,
                message = "查询成功！"
            };
            TypeAdapterConfig<Sys_Menu, SysMenuNode>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            List<Sys_Menu> sysMenus = _mySystemService.SelectMenuByUserName(username);
            if (sysMenus.Count > 0)
            {
                long rootMenuId = sysMenus.First().id;

                List<SysMenuNode> sysMenuNodes = new List<SysMenuNode>();
                foreach (Sys_Menu sys_Menu in sysMenus)
                {
                    //SysMenuNode sysMenuNode = new SysMenuNode
                    //{
                    //    id = sys_Menu.id,
                    //    menu_pid = sys_Menu.menu_pid,
                    //    menu_pids = sys_Menu.menu_pids,
                    //    is_leaf = sys_Menu.is_leaf,
                    //    menu_name = sys_Menu.menu_name,
                    //    url = sys_Menu.url,
                    //    icon = sys_Menu.icon,
                    //    sort = sys_Menu.sort,
                    //    level = sys_Menu.level,
                    //    status = sys_Menu.status,
                    //};
                    // SysMenuNode sysMenuNode = sys_Menu.Adapt<SysMenuNode>();
                    SysMenuNode sysMenuNode = sys_Menu.BuildAdapter().AdaptToType<SysMenuNode>();
                    sysMenuNodes.Add(sysMenuNode);
                }
                msg.data = DataTreeUtil<SysMenuNode, long>.BuildTreeWithoutRoot(sysMenuNodes, rootMenuId);
                return msg;
            }
            msg.data = new List<SysMenuNode>();
            return msg;
        }

        /// <summary>
        /// 菜单管理：更新菜单的禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public MsgModel UpdateStatus(long id, bool status)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "菜单禁用状态更新成功！"
            };
            Sys_Menu sys_Menu = new Sys_Menu
            {
                id = id,
                status = status
            };
            _baseSysMenuService.UpdateRange(sys_Menu);
            return msg;
        }

    }
}
