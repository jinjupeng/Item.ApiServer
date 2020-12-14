using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApiServer.DAL.IDAL
{
    public interface IBaseDal<T> where T : class
    {
        void AddRange(IEnumerable<T> t);
        void AddRange(params T[] t);
        void DeleteRange(IEnumerable<T> t);
        void DeleteRange(params T[] t);
        void UpdateRange(IEnumerable<T> t);
        void UpdateRange(params T[] t);
        IQueryable<T> ExecSql(string sql);
        int CountAll();

        IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> QueryByPage<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy);

        bool SaveChanges();
    }
}