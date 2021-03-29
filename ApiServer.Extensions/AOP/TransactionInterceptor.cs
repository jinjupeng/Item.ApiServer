using ApiServer.Common.Attributes;
using ApiServer.DAL.UnitOfWork;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ApiServer.Extensions.AOP
{
    /// <summary>
    /// 事务拦截器
    /// </summary>
    public class TransactionInterceptor : IInterceptor
    {
        /**
         * 参考：老张的哲学，关于事务的一篇文章介绍
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
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            // 判断方法是否使用了Transaction注解，如果使用了才给它加事务
            if (method.GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(TransactionAttribute)) is TransactionAttribute)
            {
                if (!_unitOfWork.IsEnabledTransaction)
                {
                    _unitOfWork.BeginTransaction();
                }
                var trans = _unitOfWork.CurrentTransaction;
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
                catch(Exception ex)
                {
                    if (trans != null)
                    {
                        _logger.LogInformation(new EventId(trans.GetHashCode()), "Transaction Rollback");
                        trans.Rollback();
                    }
                    throw new Exception(ex.InnerException.Message);
                }
            }
            else
            {
                invocation.Proceed();//直接执行被拦截方法
            }
        }
    }
}
