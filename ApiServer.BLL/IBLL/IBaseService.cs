using ApiServer.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Item.ApiServer.BLL.IBLL
{
    public interface IBaseService<T> where T : class
    {
        bool AddRange(IEnumerable<T> t);
        bool AddRange(params T[] t);
        bool DeleteRange(IEnumerable<T> t);
        bool DeleteRange(params T[] t);
        bool UpdateRange(IEnumerable<T> t);
        bool UpdateRange(params T[] t);
        int CountAll();

        /// <summary>
        /// 根据whereLambda获取IQueryable集合
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> GetModels(Func<T, bool> whereLambda);
    }
}