using ApiServer.Model.Entity;
using Item.ApiServer.BLL.IBLL;
using Item.ApiServer.DAL.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Item.ApiServer.BLL.BLL
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IBaseDal<T> _baseDal;

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

        public IQueryable<T> GetModels(Func<T, bool> whereLambda)
        {
            return _baseDal.GetModels(whereLambda);
        }
    }
}