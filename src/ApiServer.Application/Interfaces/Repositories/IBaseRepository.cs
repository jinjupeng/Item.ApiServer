using ApiServer.Domain.Common;
using System.Linq.Expressions;

namespace ApiServer.Application.Interfaces.Repositories
{
    /// <summary>
    /// 基础仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IBaseRepository<T> where T : BaseEntity
    {
        #region 查询操作

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        Task<T?> GetByIdAsync(long id);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// 根据条件查询
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件查询单个实体
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取实体数量
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null);

        #endregion

        #region 添加操作

        /// <summary>
        /// 添加实体
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        Task UpdateRangeAsync(IEnumerable<T> entities);

        #endregion

        #region 删除操作

        /// <summary>
        /// 删除实体（物理删除）
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// 根据ID删除实体（物理删除）
        /// </summary>
        Task DeleteAsync(long id);

        /// <summary>
        /// 批量删除实体（物理删除）
        /// </summary>
        Task DeleteRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// 软删除实体
        /// </summary>
        Task SoftDeleteAsync(long id);

        /// <summary>
        /// 批量软删除实体
        /// </summary>
        Task SoftDeleteRangeAsync(IEnumerable<long> ids);

        #endregion

        #region 查询构建器

        /// <summary>
        /// 获取查询构建器
        /// </summary>
        IQueryable<T> GetQueryable();

        /// <summary>
        /// 获取包含导航属性的查询构建器
        /// </summary>
        IQueryable<T> GetQueryableWithInclude(params Expression<Func<T, object>>[] includes);

        #endregion
    }
}