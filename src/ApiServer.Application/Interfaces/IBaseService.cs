using ApiServer.Domain.Entities;
using ApiServer.Shared.Common;
using System.Linq.Expressions;

namespace ApiServer.Application.Interfaces
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IBaseService<TEntity> where TEntity : class
    {
        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件获取实体列表
        /// </summary>
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 分页获取实体列表
        /// </summary>
        Task<PagedResult<TEntity>> GetPagedListAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取实体数量
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加实体
        /// </summary>
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        Task<int> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新实体
        /// </summary>
        Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除实体
        /// </summary>
        Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除实体
        /// </summary>
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量删除实体
        /// </summary>
        Task<int> DeleteRangeAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}