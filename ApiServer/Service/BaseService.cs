﻿using ApiServer.DAL.DAL;
using ApiServer.Models.Model.PageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnitOfWork;

namespace ApiServer.BLL.BLL
{
    public interface IBaseService<T> where T : class
    {
        bool AddRange(IEnumerable<T> t);
        bool AddRange(params T[] t);
        bool DeleteRange(IEnumerable<T> t);
        bool DeleteRange(params T[] t);
        bool UpdateRange(IEnumerable<T> t);
        bool UpdateRange(params T[] t);

        /// <summary>
        /// 根据whereLambda获取IQueryable集合
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda);

        PageModel<T> QueryByPage<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy);

        Task<int> Insert(T entity);

        Task<int> Update(T entity);

        Task<bool> IsExist(Expression<Func<T, bool>> whereLambda);

        Task<T> GetEntity(Expression<Func<T, bool>> whereLambda);

        Task<List<T>> Select();

        Task<List<T>> Select(Expression<Func<T, bool>> whereLambda);

        Task<Tuple<List<T>, int>> Select<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc);
    }


    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IBaseDal<T> _baseDal;
        protected IUnitOfWork unitOfWork;

        public BaseService() { }
        public BaseService(IBaseDal<T> baseDal, IUnitOfWork unitOfWork)
        {
            _baseDal = baseDal;
            this.unitOfWork = unitOfWork;
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

        public async Task<int> Insert(T entity)
        {
            await _baseDal.Insert(entity);
            return await unitOfWork.SaveChangesAsync();
        }

        public async Task<int> Update(T entity)
        {
            _baseDal.Update(entity);
            return await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsExist(Expression<Func<T, bool>> whereLambda)
        {
            return await _baseDal.IsExist(whereLambda);
        }

        public async Task<T> GetEntity(Expression<Func<T, bool>> whereLambda)
        {
            return await _baseDal.GetEntity(whereLambda);
        }

        public async Task<List<T>> Select()
        {
            return await _baseDal.Select();
        }

        public async Task<List<T>> Select(Expression<Func<T, bool>> whereLambda)
        {
            return await _baseDal.Select(whereLambda);
        }

        public async Task<Tuple<List<T>, int>> Select<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc)
        {
            return await _baseDal.Select(pageSize, pageIndex, whereLambda, orderByLambda, isAsc);
        }
    }
}