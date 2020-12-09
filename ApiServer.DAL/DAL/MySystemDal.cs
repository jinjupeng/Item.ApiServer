using ApiServer.DAL.IDAL;
using ApiServer.Model.Entity;
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
            FormattableString sql = $"SELECT distinct role_id FROM sys_user_role ra WHERE ra.user_id = {userId};";
            return DbContext.Set<string>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();

        }

        public int InsertRoleApiIds(long roleId, List<long> checkedIds)
        {
            throw new System.NotImplementedException();
        }

        public int InsertRoleMenuIds(long roleId, List<long> checkedIds)
        {
            throw new System.NotImplementedException();
        }

        public long InsertUserRoleIds(long userId, List<long> checkedIds)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<string> SelectApiCheckedKeys(long roleId)
        {
            FormattableString sql = $"SELECT distinct api_id FROM sys_role_api ra WHERE role_id = {roleId}";
            return DbContext.Set<string>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();
        }

        public IQueryable<string> SelectApiExpandedKeys()
        {
            string sql = "SELECT distinct id FROM sys_api a WHERE a.level = 2";
            return DbContext.Set<string>().FromSqlRaw(sql).AsNoTracking().AsQueryable();
        }

        public IQueryable<Sys_Api> SelectApiTree(long rootApiId, string apiNameLike, bool apiStatus)
        {
            string sql = $"SELECT id,api_pid,api_pids,is_leaf,api_name,url,sort,level,status FROM sys_api o " +
            $"WHERE (api_pids like CONCAT('%[',{rootApiId},']%') OR id = {rootApiId}) ";
            if (apiNameLike != null && apiNameLike != "")
            {
                sql += $"AND api_name like CONCAT('%',{apiNameLike},'%') ";
            }

            sql += $"AND status = {apiStatus} ";
            sql += $"ORDER BY level,sort";

            return DbContext.Set<Sys_Api>().FromSqlRaw(sql).AsNoTracking().AsQueryable();

        }

        public IQueryable<Sys_Menu> SelectMenuByUserName(string userName)
        {
            FormattableString sql = $@"
            SELECT distinct m.id,menu_pid,menu_pids,is_leaf,menu_name,url,icon,sort,level,status
            FROM sys_menu m
            LEFT JOIN sys_role_menu rm ON m.id = rm.menu_id
            LEFT JOIN sys_user_role ur ON ur.role_id = rm.role_id
            LEFT JOIN sys_user u ON u.id = ur.user_id
            WHERE u.username = {userName}
            AND m.status = 0
            ORDER BY level,sort
";
            return DbContext.Set<Sys_Menu>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();

        }

        public IQueryable<string> SelectMenuCheckedKeys(long roleId)
        {
            FormattableString sql = $"SELECT distinct menu_id FROM sys_role_menu ra WHERE ra.role_id = {roleId}";
            return DbContext.Set<string>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();

        }

        public IQueryable<string> SelectMenuExpandedKeys()
        {
            FormattableString sql = $"SELECT distinct id FROM sys_menu a WHERE a.level = 2";
            return DbContext.Set<string>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();
        }

        public IQueryable<Sys_Menu> SelectMenuTree(long rootMenuId, string menuNameLike, bool menuStatus)
        {
            string sql = $"SELECT id,menu_pid,menu_pids,is_leaf,menu_name,url,icon,sort,level,status FROM sys_menu o " +
            $"WHERE (menu_pids like CONCAT('%[',{rootMenuId},']%') OR id = {rootMenuId}) ";
            if (menuNameLike != null && menuNameLike != "")
            {
                sql += $"AND menu_name like CONCAT('%',{menuNameLike},'%') ";
            }

            sql += $"AND status = {menuStatus} ";
            sql += $"ORDER BY level,sort";

            return DbContext.Set<Sys_Menu>().FromSqlRaw(sql).AsNoTracking().AsQueryable();

        }

        public IQueryable<Sys_Org> SelectOrgTree(long rootOrgId, string orgNameLike, bool orgStatus)
        {
            string sql = $"SELECT id,org_pid,org_pids,is_leaf,org_name,address,phone,email,sort,level,status FROM sys_org o " +
            $"WHERE (org_pids like CONCAT('%[',{rootOrgId},']%') OR id = {rootOrgId}) ";
            if (orgNameLike != null && orgNameLike != "")
            {
                sql += $"AND org_name like CONCAT('%',{orgNameLike},'%') ";
            }

            sql += $"AND status = {orgStatus} ";
            sql += $"ORDER BY level,sort";

            return DbContext.Set<Sys_Org>().FromSqlRaw(sql).AsNoTracking().AsQueryable();

        }

        IQueryable<string> IMySystemDal.GetCheckedRoleIds(long userId)
        {
            FormattableString sql = $"SELECT distinct role_id FROM sys_user_role ra WHERE ra.user_id = {userId};";
            return DbContext.Set<string>().FromSqlInterpolated(sql).AsNoTracking().AsQueryable();

        }
    }
}
