using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Infrastructure.Data;
using ApiServer.Infrastructure.Repositories;

namespace ApiServer.Infrastructure.UnitOfWork
{
    /// <summary>
    /// 工作单元实现
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed = false;

        // 仓储缓存
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取仓储实例
        /// </summary>
        /// <typeparam name="TRepository">仓储接口类型</typeparam>
        /// <returns>仓储实例</returns>
        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            var repositoryType = typeof(TRepository);

            if (_repositories.ContainsKey(repositoryType))
            {
                return (TRepository)_repositories[repositoryType];
            }

            // 根据接口类型创建对应的仓储实现
            object repository = repositoryType.Name switch
            {
                nameof(IUserRepository) => new UserRepository(_context),
                // 可以在这里添加其他特定仓储的映射
                _ => throw new ArgumentException($"Repository type {repositoryType.Name} is not supported")
            };

            _repositories.Add(repositoryType, repository);
            return (TRepository)repository;
        }

        /// <summary>
        /// 获取基础仓储实例
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>基础仓储实例</returns>
        public IBaseRepository<T> GetBaseRepository<T>() where T : Domain.Common.BaseEntity
        {
            var repositoryType = typeof(IBaseRepository<T>);

            if (_repositories.ContainsKey(repositoryType))
            {
                return (IBaseRepository<T>)_repositories[repositoryType];
            }

            var repository = new BaseRepository<T>(_context);
            _repositories.Add(repositoryType, repository);
            return repository;
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (_context.Database.CurrentTransaction == null)
            {
                await _context.Database.BeginTransactionAsync();
            }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            var transaction = _context.Database.CurrentTransaction;
            if (transaction != null)
            {
                try
                {
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                finally
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            var transaction = _context.Database.CurrentTransaction;
            if (transaction != null)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                finally
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// 执行事务操作
        /// </summary>
        /// <param name="action">事务操作</param>
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            await BeginTransactionAsync();
            try
            {
                await action();
                await CommitTransactionAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// 执行事务操作并返回结果
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">事务操作</param>
        /// <returns>操作结果</returns>
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func)
        {
            await BeginTransactionAsync();
            try
            {
                var result = await func();
                await CommitTransactionAsync();
                return result;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        }

        #region IDisposable

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放仓储缓存
                    foreach (var repository in _repositories.Values)
                    {
                        if (repository is IDisposable disposableRepository)
                        {
                            disposableRepository.Dispose();
                        }
                    }
                    _repositories.Clear();

                    // 释放上下文
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}