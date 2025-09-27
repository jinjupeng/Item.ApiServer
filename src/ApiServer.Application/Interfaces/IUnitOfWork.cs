using ApiServer.Application.Interfaces.Repositories;

namespace ApiServer.Application.Interfaces
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 获取仓储实例
        /// </summary>
        /// <typeparam name="TRepository">仓储接口类型</typeparam>
        /// <returns>仓储实例</returns>
        TRepository GetRepository<TRepository>() where TRepository : class;

        /// <summary>
        /// 获取基础仓储实例
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>基础仓储实例</returns>
        IBaseRepository<T> GetBaseRepository<T>() where T : Domain.Common.BaseEntity;

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// 开始事务
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// 提交事务
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// 回滚事务
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// 执行事务操作
        /// </summary>
        /// <param name="action">事务操作</param>
        Task ExecuteInTransactionAsync(Func<Task> action);

        /// <summary>
        /// 执行事务操作并返回结果
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">事务操作</param>
        /// <returns>操作结果</returns>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func);
    }
}