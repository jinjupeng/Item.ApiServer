using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Domain.Common;
using ApiServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiServer.Infrastructure.Repositories
{
    /// <summary>
    /// 基础仓储实现
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        #region 查询操作

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = _dbSet.AsQueryable();
            if (typeof(SoftDeleteEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((SoftDeleteEntity)(object)e).IsDeleted);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _dbSet.AsQueryable();
            if (typeof(SoftDeleteEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((SoftDeleteEntity)(object)e).IsDeleted);
            }
            return await query.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _dbSet.AsQueryable();
            if (typeof(SoftDeleteEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((SoftDeleteEntity)(object)e).IsDeleted);
            }
            return await query.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _dbSet.AsQueryable();
            if (typeof(SoftDeleteEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((SoftDeleteEntity)(object)e).IsDeleted);
            }
            return await query.AnyAsync(predicate);
        }

        /// <summary>
        /// 获取实体数量
        /// </summary>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var query = _dbSet.AsQueryable();
            if (typeof(SoftDeleteEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((SoftDeleteEntity)(object)e).IsDeleted);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.CountAsync();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null)
        {
            var query = _dbSet.AsQueryable();
            if (typeof(SoftDeleteEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((SoftDeleteEntity)(object)e).IsDeleted);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        #endregion

        #region 添加操作

        /// <summary>
        /// 添加实体
        /// </summary>
        public virtual async Task<T> AddAsync(T entity)
        {
            entity.CreateTime = DateTime.Now;
            if (entity is SoftDeleteEntity softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = false;
            }
            
            await _dbSet.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// 批量添加实体
        /// </summary>
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            var now = DateTime.Now;
            foreach (var entity in entities)
            {
                entity.CreateTime = now;
                if (entity is SoftDeleteEntity softDeleteEntity)
                {
                    softDeleteEntity.IsDeleted = false;
                }
            }
            
            await _dbSet.AddRangeAsync(entities);
        }

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体
        /// </summary>
        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        public virtual Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        #endregion

        #region 删除操作

        /// <summary>
        /// 删除实体（物理删除）
        /// </summary>
        public virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 根据ID删除实体（物理删除）
        /// </summary>
        public virtual async Task DeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        /// <summary>
        /// 批量删除实体（物理删除）
        /// </summary>
        public virtual Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 软删除实体
        /// </summary>
        public virtual async Task SoftDeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null && entity is SoftDeleteEntity softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                softDeleteEntity.DeletedTime = DateTime.Now;
                await UpdateAsync(entity);
            }
        }

        /// <summary>
        /// 批量软删除实体
        /// </summary>
        public virtual async Task SoftDeleteRangeAsync(IEnumerable<long> ids)
        {
            var entities = await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
            var now = DateTime.Now;
            foreach (var entity in entities)
            {
                if (entity is SoftDeleteEntity softDeleteEntity)
                {
                    softDeleteEntity.IsDeleted = true;
                    softDeleteEntity.DeletedTime = now;
                }
            }
            await UpdateRangeAsync(entities);
        }

        #endregion

        #region 查询构建器

        /// <summary>
        /// 获取查询构建器
        /// </summary>
        public virtual IQueryable<T> GetQueryable()
        {
            var query = _dbSet.AsQueryable();
            if (typeof(SoftDeleteEntity).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !((SoftDeleteEntity)(object)e).IsDeleted);
            }
            return query;
        }

        /// <summary>
        /// 获取包含导航属性的查询构建器
        /// </summary>
        public virtual IQueryable<T> GetQueryableWithInclude(params Expression<Func<T, object>>[] includes)
        {
            var query = GetQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        #endregion
    }
}