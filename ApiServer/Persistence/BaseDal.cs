﻿using ApiServer.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ApiServer.DAL.DAL
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

        IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> QueryByPage<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderBy);

        bool SaveChanges();

        T Add(T entity);

        T Update(T entity);

        bool Remove(T entity);
        ValueTask<EntityEntry<T>> Insert(T entity);

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task<bool> RemoveAsync(T entity);

        Task<bool> IsExist(Expression<Func<T, bool>> whereLambda);

        Task<T> GetEntity(Expression<Func<T, bool>> whereLambda);

        Task<List<T>> Select();

        Task<List<T>> Select(Expression<Func<T, bool>> whereLambda);

        Task<Tuple<List<T>, int>> Select<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc);
    }
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

        public async ValueTask<EntityEntry<T>> Insert(T entity)
        {
            return await _context.Set<T>().AddAsync(entity);
        }


        public async Task<bool> IsExist(Expression<Func<T, bool>> whereLambda)
        {
            return await _context.Set<T>().AnyAsync(whereLambda);
        }

        public async Task<T> GetEntity(Expression<Func<T, bool>> whereLambda)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(whereLambda);
        }

        public async Task<List<T>> Select()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<List<T>> Select(Expression<Func<T, bool>> whereLambda)
        {
            return await _context.Set<T>().Where(whereLambda).ToListAsync();
        }

        public async Task<Tuple<List<T>, int>> Select<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc)
        {
            var total = await _context.Set<T>().Where(whereLambda).CountAsync();

            if (isAsc)
            {
                var entities = await _context.Set<T>().Where(whereLambda)
                                      .OrderBy<T, S>(orderByLambda)
                                      .Skip(pageSize * (pageIndex - 1))
                                      .Take(pageSize).ToListAsync();

                return new Tuple<List<T>, int>(entities, total);
            }
            else
            {
                var entities = await _context.Set<T>().Where(whereLambda)
                                      .OrderByDescending<T, S>(orderByLambda)
                                      .Skip(pageSize * (pageIndex - 1))
                                      .Take(pageSize).ToListAsync();

                return new Tuple<List<T>, int>(entities, total);
            }
        }

        public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Add(entity));
        }

        public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Update(entity));
        }

        public Task<bool> RemoveAsync(T entity)
        {
            return Task.FromResult(Remove(entity));
        }

        public T Add(T entity)
        {
            return _context.Set<T>().Add(entity).Entity;
        }

        public T Update(T entity)
        {
            return _context.Set<T>().Update(entity).Entity;
        }

        public bool Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
            return true;
        }
    }
}