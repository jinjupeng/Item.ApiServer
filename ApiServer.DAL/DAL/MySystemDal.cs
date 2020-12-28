using ApiServer.DAL.IDAL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiServer.DAL.DAL
{
    public class MySystemDal : IMySystemDal
    {
        public readonly DbContext DbContext = (new ContextProvider()).GetContext();


        public IQueryable<string> GetCheckedRoleIds(long userId)
        {
            // FormattableString sql = $"SELECT distinct role_id FROM Sys_User_Role ra WHERE ra.user_id = {userId};";
            // var userRoles = DbContext.Set<Sys_User_Role>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();
            var userRoles = DbContext.Set<Sys_User_Role>().Where(a => a.user_id == userId);
            var list = new List<string>();
            foreach (var userRole in userRoles)
            {
                list.Add(Convert.ToString(userRole.role_id));
            }
            return list.AsQueryable();

        }

        public int InsertRoleApiIds(long roleId, List<long> checkedIds)
        {
            foreach (var checkedId in checkedIds)
            {
                string sql = $"INSERT INTO Sys_Role_Api (role_id, api_id) VALUES({roleId}, {checkedId})";
                DbContext.Database.ExecuteSqlRaw(sql);
            }
           
            return DbContext.SaveChanges();
        }

        public int InsertRoleMenuIds(long roleId, List<long> checkedIds)
        {
            foreach (var checkedId in checkedIds)
            {
                string sql = $"INSERT INTO Sys_Role_Menu (role_id, menu_id) VALUES({roleId}, {checkedId})";
                DbContext.Database.ExecuteSqlRaw(sql);
            }
            return DbContext.SaveChanges();
        }

        public long InsertUserRoleIds(long userId, List<long> checkedIds)
        {
            foreach (var checkedId in checkedIds)
            {
                string sql = $"INSERT INTO Sys_User_Role (role_id, user_id) VALUES({userId}, {checkedId})";
                DbContext.Database.ExecuteSqlRaw(sql);
            }
            return DbContext.SaveChanges();
        }

        public IQueryable<string> SelectApiCheckedKeys(long roleId)
        {
            // FormattableString sql = $"SELECT distinct api_id FROM Sys_Role_api ra WHERE role_id = {roleId}";
            // var roleApis = DbContext.Set<Sys_Role_Api>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();
            var roleApis = DbContext.Set<Sys_Role_Api>().Where(a => a.role_id == roleId);
            var list = new List<string>();
            foreach (var sysRoleApi in roleApis)
            {
                list.Add(Convert.ToString(sysRoleApi.api_id));
            }
            return list.AsQueryable();
        }

        public IQueryable<string> SelectApiExpandedKeys()
        {
            // string sql = "SELECT distinct id FROM Sys_Api a WHERE a.level = 2";
            // var sysApis = DbContext.Set<Sys_Api>().FromSqlRaw(sql).AsNoTracking().AsQueryable();
            var sysApis = DbContext.Set<Sys_Api>().Where(a => a.level == 2);
            var list = new List<string>();
            foreach (var sysApi in sysApis)
            {
                list.Add(Convert.ToString(sysApi.id));
            }
            return list.AsQueryable();
        }

        public IQueryable<Sys_Api> SelectApiTree(long rootApiId, string apiNameLike, bool apiStatus)
        {
            string sql = $"SELECT id,api_pid,api_pids,is_leaf,api_name,url,sort,level,status FROM Sys_Api o " +
            $"WHERE (api_pids like CONCAT('%[{rootApiId}]%') OR id = {rootApiId}) ";
            if (apiNameLike != null && apiNameLike != "")
            {
                sql += $"AND api_name like CONCAT('%{apiNameLike}%') ";
            }

            sql += $"AND status = {(apiStatus ? 1 : 0)} ";
            sql += $"ORDER BY level,sort";

            return DbContext.Set<Sys_Api>().FromSqlRaw(sql).AsNoTracking().AsQueryable();

        }

        public IQueryable<Sys_Menu> SelectMenuByUserName(string userName)
        {
            FormattableString sql = $@"
            SELECT distinct m.id,menu_pid,menu_pids,is_leaf,menu_name,url,icon,sort,level,status
            FROM Sys_Menu m
            LEFT JOIN Sys_Role_Menu rm ON m.id = rm.menu_id
            LEFT JOIN Sys_User_Role ur ON ur.role_id = rm.role_id
            LEFT JOIN Sys_User u ON u.id = ur.user_id
            WHERE u.username = {userName}
            AND m.status = 0
            ORDER BY level,sort
            ";
            return DbContext.Set<Sys_Menu>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();

        }

        public IQueryable<string> SelectMenuCheckedKeys(long roleId)
        {
            // FormattableString sql = $"SELECT distinct menu_id FROM Sys_Role_Menu ra WHERE ra.role_id = {roleId}";
            // var roleMenus = DbContext.Set<Sys_Role_Menu>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();
            var roleMenus = DbContext.Set<Sys_Role_Menu>().Where(a => a.role_id == roleId);
            var list = new List<string>();
            foreach (var roleMenu in roleMenus)
            {
                list.Add(Convert.ToString(roleMenu.menu_id));
            }
            return list.AsQueryable();

        }

        public IQueryable<string> SelectMenuExpandedKeys()
        {
            // FormattableString sql = $"SELECT distinct id FROM Sys_Menu a WHERE a.level = 2";
            // var sysMenus = DbContext.Set<Sys_Menu>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();
            var sysMenus = DbContext.Set<Sys_Menu>().Where(a => a.level == 2);
            var list = new List<string>();
            foreach (var sysMenu in sysMenus)
            {
                list.Add(Convert.ToString(sysMenu.id));
            }
            return list.AsQueryable();
        }

        public IQueryable<Sys_Menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool? menuStatus)
        {
            string sql = $"SELECT id,menu_pid,menu_pids,is_leaf,menu_name,url,icon,sort,level,status FROM Sys_Menu o " +
            $"WHERE (menu_pids like CONCAT('%[{rootMenuId}]%') OR id = {rootMenuId}) ";
            if (menuNameLike != null && menuNameLike != "")
            {
                sql += $"AND menu_name like CONCAT('%{menuNameLike}%') ";
            }
            if (menuStatus != null)
            {
                sql += $"AND status = {((bool)menuStatus ? 1 : 0)} ";
            }
            sql += $"ORDER BY level,sort";

            return DbContext.Set<Sys_Menu>().FromSqlRaw(sql).AsNoTracking().AsQueryable();

        }

        public IQueryable<Sys_Org> SelectOrgTree(long rootOrgId, string orgNameLike, bool? orgStatus)
        {
            string sql = $"SELECT id,org_pid,org_pids,is_leaf,org_name,address,phone,email,sort,level,status FROM Sys_Org o " +
            $"WHERE (org_pids like CONCAT('%[{rootOrgId}]%') OR id = {rootOrgId}) ";
            if (orgNameLike != null && orgNameLike != "")
            {
                sql += $"AND org_name like CONCAT('%{orgNameLike}%') ";
            }
            if (orgStatus != null)
            {
                sql += $"AND status = {((bool)orgStatus ? 1 : 0)} ";
            }

            sql += $"ORDER BY level,sort";

            return DbContext.Set<Sys_Org>().FromSqlRaw(sql).AsNoTracking().AsQueryable();

        }

        //public IQueryable<string> GetCheckedRoleIds(long userId)
        //{
        //    // FormattableString sql = $"SELECT distinct role_id FROM Sys_User_Role ra WHERE ra.user_id = {userId};";
        //    // return DbContext.Set<string>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();
        //    var sysMenus = DbContext.Set<Sys_User_Role>().Where(a => a.user_id == userId);
        //    var list = new List<string>();
        //    foreach (var sysMenu in sysMenus)
        //    {
        //        list.Add(Convert.ToString(sysMenu.role_id));
        //    }
        //    return list.AsQueryable();
        //}

        public IQueryable<SysUserOrg> SelectUser(long? orgId,
                                      string userName,
                                      string phone,
                                      string email,
                                      bool? enabled,
                                      DateTime? createStartTime,
                                      DateTime? createEndTime)
        {
            // https://www.cnblogs.com/wanghaibin/p/6494309.html

            var userList = DbContext.Set<Sys_User>().AsNoTracking();
            var orgList = DbContext.Set<Sys_Org>().AsNoTracking();
            var userOrgList = (from u in userList
                               join o in orgList on u.org_id equals o.id
                               where string.IsNullOrEmpty(userName) || u.username.Contains(userName)
                               where string.IsNullOrEmpty(phone) || u.phone.Contains(phone)
                               where string.IsNullOrEmpty(email) || u.email.Contains(email)
                               where enabled == null || u.enabled == enabled
                               where createStartTime == null || createEndTime == null || u.create_time >= createStartTime && u.create_time <= createEndTime
                               where orgId == null || o.id == orgId || o.org_pids.Contains("[" + orgId + "]")
                               select new SysUserOrg
                               {
                                   id = u.id,
                                   username = u.username,
                                   org_id = u.org_id,
                                   OrgName = o.org_name,
                                   enabled = u.enabled,
                                   phone = u.phone,
                                   email = u.email,
                                   create_time = u.create_time
                               });

            return userOrgList;
            //string sql = @"SELECT u.id,u.username,u.org_id,o.org_name,u.enabled,u.phone,u.email,u.create_time
            //            FROM Sys_User u
            //            LEFT JOIN Sys_Org o ON u.org_id = o.id";

            //var result = DbContext.Set<SysUserOrg>().FromSqlRaw(sql).AsNoTracking().AsQueryable();
            //if (!string.IsNullOrWhiteSpace(userName))
            //{
            //    result = result.Where(a => a.username.Contains(userName));
            //}
            //if (!string.IsNullOrWhiteSpace(phone))
            //{
            //    result = result.Where(a => a.phone.Contains(phone));
            //}
            //if (!string.IsNullOrWhiteSpace(email))
            //{
            //    result = result.Where(a => a.email.Contains(email));
            //}
            //if (enabled != null)
            //{
            //    result = result.Where(a => a.enabled == enabled);
            //}
            //if (createStartTime != null && createEndTime != null)
            //{
            //    result = result.Where(a => a.create_time >= createStartTime && a.create_time <= createEndTime);
            //}
            //if (orgId != null)
            //{
            //    result = result.Where(a => a.id == orgId || a.org_pids.Contains("[" + orgId + "]"));
            //}

        }
    }
}
