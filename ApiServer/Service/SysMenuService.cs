﻿using ApiServer.Models.Entity;
using ApiServer.Models.Model.DataTree;
using ApiServer.Models.Model.MsgModel;
using ApiServer.Models.Model.Nodes;
using Common.Utility.Utils;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using UnitOfWork;

namespace ApiServer.BLL.BLL
{
    public interface ISysMenuService
    {
        MsgModel GetMenuTree(string menuNameLike, bool? menuStatus);
        MsgModel UpdateMenu(sys_menu sys_menu);
        MsgModel AddMenu(sys_menu sys_menu);
        MsgModel DeleteMenu(sys_menu sys_menu);
        List<string> GetCheckedKeys(long roleId);
        List<string> GetExpandedKeys();
        MsgModel SaveCheckedKeys(long roleId, List<long> checkedIds);
        MsgModel GetMenuTreeByUsername(string username);
        MsgModel UpdateStatus(long id, bool status);
    }
    public class SysMenuService : ISysMenuService
    {
        private readonly IBaseService<sys_menu> _baseSysMenuService;
        private readonly IMySystemService _mySystemService;
        private readonly IBaseService<sys_role_menu> _baseSysRoleMenuService;

        public SysMenuService(IBaseService<sys_menu> baseSysMenuService, IMySystemService mySystemService, IBaseService<sys_role_menu> baseSysRoleMenuService)
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

            //保证数据库里面level=1的节点只有一个，根节点
            sys_menu rootSysMenu = _baseSysMenuService.GetModels(a => a.level == 1).SingleOrDefault();
            if (rootSysMenu != null)
            {
                long rootMenuId = rootSysMenu.id;
                List<sys_menu> sysMenus = _mySystemService.SelectMenuTree(rootMenuId, menuNameLike, menuStatus);
                List<SysMenuNode> sysMenuNodes = new List<SysMenuNode>();
                foreach (sys_menu sys_menu in sysMenus)
                {
                    SysMenuNode sysMenuNode = sys_menu.BuildAdapter().AdaptToType<SysMenuNode>();
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
                msg.message = "请先在数据库内为菜单配置一个分类的根节点，level=1";
                msg.data = new List<SysMenuNode>();
                return msg;
            }
        }

        public MsgModel UpdateMenu(sys_menu sys_menu)
        {
            _baseSysMenuService.UpdateRange(sys_menu);
            return MsgModel.Success("更新菜单项成功！");
        }

        [Transaction]
        public MsgModel AddMenu(sys_menu sys_menu)
        {
            sys_menu.id = new Snowflake().GetId();
            SetMenuIdsAndLevel(sys_menu);
            sys_menu.is_leaf = true;//新增的菜单节点都是子节点，没有下级

            sys_menu parent = _baseSysMenuService.GetModels(a => a.id == sys_menu.menu_pid).SingleOrDefault();
            parent.id = sys_menu.menu_pid;
            parent.is_leaf = false; //更新父节点为非子节点。
            _baseSysMenuService.UpdateRange(parent);

            sys_menu.status = false;//设置是否禁用，新增节点默认可用
            _baseSysMenuService.AddRange(sys_menu);
            return MsgModel.Success("新增菜单项成功！");
        }

        [Transaction]
        public MsgModel DeleteMenu(sys_menu sys_menu)
        {
            //查找被删除节点的子节点
            List<sys_menu> myChilds = _baseSysMenuService.GetModels(a => a.menu_pids.Contains("[" + sys_menu.id + "]")).ToList();

            if (myChilds.Count > 0)
            {
                // "不能删除含有下级菜单的菜单"
                return MsgModel.Fail("不能删除含有下级菜单的菜单");
            }
            //查找被删除节点的父节点
            List<sys_menu> myFatherChilds = _baseSysMenuService.GetModels(a => a.menu_pids.Contains("[" + sys_menu.menu_pid + "]")).ToList();

            //我的父节点只有我这一个子节点，而我还要被删除，更新父节点为叶子节点。
            if (myFatherChilds.Count == 1)
            {
                sys_menu parent = _baseSysMenuService.GetModels(a => a.id == sys_menu.menu_pid).SingleOrDefault();
                parent.id = sys_menu.menu_pid;
                parent.is_leaf = true;//更新父节点为叶子节点。
                _baseSysMenuService.UpdateRange(parent);
            }
            // 删除节点
            _baseSysMenuService.DeleteRange(sys_menu);
            return MsgModel.Success("删除菜单项成功！");
        }

        /// <summary>
        /// 设置某子节点的所有祖辈id
        /// </summary>
        /// <param name="child"></param>
        private void SetMenuIdsAndLevel(sys_menu child)
        {
            List<sys_menu> allMenus = _baseSysMenuService.GetModels(null).ToList();
            foreach (sys_menu sys_menu in allMenus)
            {
                // 从组织列表中找到自己的直接父亲
                if (sys_menu.id == child.menu_pid)
                {
                    //直接父亲的所有祖辈id + 直接父id = 当前子节点的所有祖辈id
                    //爸爸的所有祖辈 + 爸爸 = 孩子的所有祖辈
                    child.menu_pids = sys_menu.menu_pids + ",[" + child.menu_pid + "]";
                    child.level = sys_menu.level + 1;
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
        [Transaction]
        public MsgModel SaveCheckedKeys(long roleId, List<long> checkedIds)
        {
            // 保存之前先删除
            _baseSysRoleMenuService.DeleteRange(_baseSysRoleMenuService.GetModels(a => a.role_id == roleId).ToList());
            _mySystemService.InsertRoleMenuIds(roleId, checkedIds);
            return MsgModel.Success("保存菜单权限成功！");
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
            List<sys_menu> sysMenus = _mySystemService.SelectMenuByUserName(username);
            if (sysMenus.Count > 0)
            {
                long rootMenuId = sysMenus.First().id;

                List<SysMenuNode> sysMenuNodes = new List<SysMenuNode>();
                foreach (sys_menu sys_menu in sysMenus)
                {
                    SysMenuNode sysMenuNode = sys_menu.BuildAdapter().AdaptToType<SysMenuNode>();
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
            sys_menu sys_menu = _baseSysMenuService.GetModels(a => a.id == id).SingleOrDefault();
            sys_menu.id = id;
            sys_menu.status = status;
            var result = _baseSysMenuService.UpdateRange(sys_menu);
            return result ? MsgModel.Success("菜单禁用状态更新成功！") : MsgModel.Fail("菜单禁用状态更新失败！");
        }

    }
}
