﻿using ApiServer.Model.Entity;
using Item.ApiServer.DAL.IDAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Item.ApiServer.DAL.DAL
{
    public class BaseDal<T> : IBaseDal<T> where T : class
    {
        public readonly DbContext DbContext = (new ContextProvider()).GetContext();

        public void AddRange(IEnumerable<T> t)
        {
            DbContext.Set<T>().AddRangeAsync(t);
        }
        public void AddRange(params T[] t)
        {
            DbContext.Set<T>().AddRangeAsync(t);
        }
        public void DeleteRange(IEnumerable<T> t)
        {
            DbContext.Set<T>().RemoveRange(t);
        }
        public void DeleteRange(params T[] t)
        {
            DbContext.Set<T>().RemoveRange(t);
        }
        public void UpdateRange(IEnumerable<T> t)
        {
            DbContext.Set<T>().UpdateRange(t);
        }
        public void UpdateRange(params T[] t)
        {
            DbContext.Set<T>().UpdateRange(t);
        }

        public IQueryable<T> ExecSql(string sql)
        {
            return DbContext.Set<T>().FromSqlRaw(sql).AsNoTracking().AsQueryable();
        }

        public int CountAll()
        {
            return DbContext.Set<T>().AsNoTracking().Count();
        }


        public IQueryable<T> GetModels(Func<T, bool> whereLambda)
        {
            return whereLambda != null ? DbContext.Set<T>().AsNoTracking().AsEnumerable().Where(whereLambda).AsQueryable().AsNoTracking() : DbContext.Set<T>().AsNoTracking().AsQueryable();
        }



        public bool SaveChanges()
        {
            return DbContext.SaveChangesAsync().Result > 0;
        }
    }
}