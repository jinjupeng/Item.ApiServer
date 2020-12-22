using ApiServer.BLL.IBLL;
using ApiServer.DAL.IDAL;
using ApiServer.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApiServer.BLL.BLL
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IBaseDal<T> _baseDal;

        public BaseService() { }
        public BaseService(IBaseDal<T> baseDal)
        {
            _baseDal = baseDal;
        }
        public bool AddRange(IEnumerable<T> t)
        {
            _baseDal.AddRange(t);
            return _baseDal.SaveChanges();
        }

        public bool AddRange(params T[] t)
        {
            _baseDal.AddRange(t);
            return _baseDal.SaveChanges();
        }


        public bool DeleteRange(IEnumerable<T> t)
        {
            _baseDal.DeleteRange(t);
            return _baseDal.SaveChanges();
        }

        public bool DeleteRange(params T[] t)
        {
            _baseDal.DeleteRange(t);
            return _baseDal.SaveChanges();
        }

        public bool UpdateRange(IEnumerable<T> t)
        {
            _baseDal.UpdateRange(t);
            return _baseDal.SaveChanges();
        }

        public bool UpdateRange(params T[] t)
        {
            _baseDal.UpdateRange(t);
            return _baseDal.SaveChanges();
        }


        public int CountAll()
        {
            return _baseDal.CountAll();
        }

        public IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda)
        {
            return _baseDal.GetModels(whereLambda);
        }

        public PageModel<T> QueryByPage<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy)
        {
            PageModel<T> pageModel = new PageModel<T>
            {
                pageNum = pageIndex,
                size = pageSize,
                records = _baseDal.QueryByPage(pageIndex, pageSize, whereLambda, orderBy).ToList()
            };
            pageModel.total = pageModel.records.Count;
            pageModel.pageSize = pageModel.total % pageSize > 0 ? pageModel.total / pageSize + 1 : pageModel.total / pageSize;

            return pageModel;
        }
    }
}