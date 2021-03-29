using ApiServer.Common.Attributes;
using ApiServer.DAL.UnitOfWork;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ApiServer.Extensions.AOP
{
    /// <summary>
    /// 事务拦截器
    /// </summary>
    public class TransactionInterceptor : IInterceptor
    {
        /**
         * [给Service套上事务](https://www.daemonwow.com/article/net-core-aop)
         * 
         */
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionInterceptor> _logger;

        public TransactionInterceptor(IUnitOfWork unitOfWork, ILogger<TransactionInterceptor> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            IDbTransaction trans = default;
            // invocation.Method用来获取我们调用方法的MethodInfo
            // 判断方法是否使用了Transaction注解，如果使用了才给他加事务
            if (invocation.Method.GetCustomAttributes(typeof(TransactionAttribute), true).Length != 0)
            {
                trans = this._unitOfWork.CurrentTransaction;
                _logger.LogInformation(new EventId(trans.GetHashCode()), "Use Transaction");
                try
                {
                    invocation.Proceed(); // 就是调用我们原本的方法
                    if (trans != null)
                    {
                        _logger.LogInformation(new EventId(trans.GetHashCode()), "Transaction Commit");
                        trans.Commit();
                    }
                }
                catch
                {
                    if (trans != null)
                    {
                        _logger.LogInformation(new EventId(trans.GetHashCode()), "Transaction Rollback");
                        trans.Rollback();
                    }
                    throw;
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }
    }
}
