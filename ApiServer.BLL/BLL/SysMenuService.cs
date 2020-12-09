﻿using ApiServer.BLL.IBLL;
using ApiServer.Model;
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

        /// <summary>
        /// 菜单管理：根据查询条件查询树形结构菜单列表
        /// </summary>
        /// <param name="menuNameLike">菜单名称</param>
        /// <param name="menuStatus">菜单可用状态</param>
        /// <returns>菜单列表或树形列表</returns>
        public List<SysMenuNode> GetMenuTree(string menuNameLike, bool menuStatus)
        {
            //保证数据库里面level=1的节点只有一个，根节点
            Sys_Menu rootSysMenu = _baseSysMenuService.GetModels(a => a.level == 1).Single();
            if (rootSysMenu != null)
            {
                long rootMenuId = rootSysMenu.id;
                List<Sys_Menu> sysMenus = _mySystemService.SelectMenuTree(rootMenuId, menuNameLike, menuStatus);

                List<SysMenuNode> sysMenuNodes = new List<SysMenuNode>();
                foreach (Sys_Menu sys_Menu in sysMenus)
                {
                    SysMenuNode sysMenuNode = new SysMenuNode
                    {
                        id = sys_Menu.id,
                        menu_pid = sys_Menu.id,
                        menu_pids = sys_Menu.menu_pids,
                        is_leaf = sys_Menu.is_leaf,
                        menu_name = sys_Menu.menu_name,
                        url = sys_Menu.url,
                        icon = sys_Menu.icon,
                        sort = sys_Menu.sort,
                        level = sys_Menu.level,
                        status = sys_Menu.status,
                    };
                    sysMenuNodes.Add(sysMenuNode);
                }
                if (!string.IsNullOrEmpty(menuNameLike))
                {
                    // 根据菜单名称查询，返回平面列表
                    return sysMenuNodes;
                }
                else
                {
                    // 否则返回菜单的树型结构列表
                    return DataTreeUtil<SysMenuNode, long>.BuildTree(sysMenuNodes, rootMenuId);
                }
            }
            else
            {
                // "请先在数据库内为菜单配置一个分类的根节点，level=1"
                return new List<SysMenuNode>();
            }
        }

        public void UpdateMenu(Sys_Menu sys_Menu)
        {
            _baseSysMenuService.UpdateRange(sys_Menu);
        }

        public void AddMenu(Sys_Menu sys_Menu)
        {
            SetMenuIdsAndLevel(sys_Menu);
            sys_Menu.is_leaf = true;//新增的菜单节点都是子节点，没有下级
            Sys_Menu parent = new Sys_Menu();
            parent.id = sys_Menu.menu_pid;
            parent.is_leaf = false;//更新父节点为非子节点。
            _baseSysMenuService.UpdateRange(parent);
            sys_Menu.status = false;//设置是否禁用，新增节点默认可用
            _baseSysMenuService.AddRange(sys_Menu);
        }

        public void DeleteMenu(Sys_Menu sys_Menu)
        {
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
                Sys_Menu parent = new Sys_Menu();
                parent.id = sys_Menu.menu_pid;
                parent.is_leaf = true;//更新父节点为叶子节点。
                _baseSysMenuService.UpdateRange(parent);
            }
            // 删除节点
            _baseSysMenuService.DeleteRange(sys_Menu);

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
        public void SaveCheckedKeys(long roleId, List<long> checkedIds)
        {
            // 保存之前先删除
            // _baseSysRoleMenuService.DeleteRange(_baseSysRoleMenuService.GetModels(a => a.role_id == roleId).ToList());
            _mySystemService.InsertRoleMenuIds(roleId, checkedIds);
        }

        /// <summary>
        /// 根据某用户的用户名查询该用户可以访问的菜单项（系统左侧边栏菜单）
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<SysMenuNode> GetMenuTreeByUsername(string username)
        {
            List<Sys_Menu> sysMenus = _mySystemService.SelectMenuByUserName(username);
            if (sysMenus.Count > 0)
            {
                long rootMenuId = sysMenus.First().id;

                List<SysMenuNode> sysMenuNodes = new List<SysMenuNode>();
                foreach (Sys_Menu sys_Menu in sysMenus)
                {
                    SysMenuNode sysMenuNode = new SysMenuNode
                    {
                        id = sys_Menu.id,
                        menu_pid = sys_Menu.id,
                        menu_pids = sys_Menu.menu_pids,
                        is_leaf = sys_Menu.is_leaf,
                        menu_name = sys_Menu.menu_name,
                        url = sys_Menu.url,
                        icon = sys_Menu.icon,
                        sort = sys_Menu.sort,
                        level = sys_Menu.level,
                        status = sys_Menu.status,
                    };
                    sysMenuNodes.Add(sysMenuNode);
                }
                return DataTreeUtil<SysMenuNode, long>.BuildTreeWithoutRoot(sysMenuNodes, rootMenuId);
            }
            return new List<SysMenuNode>();
        }

        /// <summary>
        /// 菜单管理：更新菜单的禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public void UpdateStatus(long id, bool status)
        {
            Sys_Menu sys_Menu = new Sys_Menu
            {
                id = id,
                status = status
            };
            _baseSysMenuService.UpdateRange(sys_Menu);
        }
    }
}
