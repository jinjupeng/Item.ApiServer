using ApiServer.DAL.IDAL;
using ApiServer.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApiServer.DAL.DAL
{
    public class BaseDal<T> : IBaseDal<T> where T : class
    {
        /// <summary>
        /// EF上下文对象
        /// </summary>
        private readonly ContextMySql _context;

        public BaseDal(ContextMySql context)
        {
            this._context = context;
        }
        public void AddRange(IEnumerable<T> t)
        {
            _context.AddRangeAsync(t);
        }
        public void AddRange(params T[] t)
        {
            _context.AddRangeAsync(t);
        }
        public void DeleteRange(IEnumerable<T> t)
        {
            _context.RemoveRange(t);
        }
        public void DeleteRange(params T[] t)
        {
            _context.RemoveRange(t);
        }
        public void UpdateRange(IEnumerable<T> t)
        {
            _context.UpdateRange(t);
        }
        public void UpdateRange(params T[] t)
        {
            _context.UpdateRange(t);
        }

        public IQueryable<T> ExecSql(string sql)
        {
            return _context.Set<T>().FromSqlRaw(sql).AsNoTracking().AsQueryable();
        }

        public int CountAll()
        {
            return _context.Set<T>().AsNoTracking().Count();
        }


        public IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda)
        {
            return whereLambda != null ? _context.Set<T>().AsNoTracking().Where(whereLambda) : _context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> QueryByPage<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            return _context.Set<T>().Where(whereLambda.Compile()).AsQueryable().OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public bool SaveChanges()
        {
            return _context.SaveChangesAsync().Result > 0;
        }
    }
}