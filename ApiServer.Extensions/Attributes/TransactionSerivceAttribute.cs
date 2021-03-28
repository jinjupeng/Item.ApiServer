using ApiServer.Extensions.AOP;
using Autofac.Extras.DynamicProxy;

namespace ApiServer.Extensions.Attributes
{
    public class TransactionSerivceAttribute : InterceptAttribute
    {
        public TransactionSerivceAttribute() : base(typeof(TransactionInterceptor)) { }
    }
}
