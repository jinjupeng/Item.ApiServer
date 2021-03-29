using ApiServer.BLL.BLL;
using ApiServer.BLL.IBLL;
using ApiServer.DAL.DAL;
using ApiServer.DAL.IDAL;
using ApiServer.DAL.UnitOfWork;
using ApiServer.Extensions.AOP;
using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using Module = Autofac.Module;

namespace ApiServer.Extensions.AutofacModule
{
    /// <summary>
    /// 多个模块注入
    /// </summary>
    public class ModuleRegister : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                var assemblys = AppDomain.CurrentDomain.GetAssemblies();
                var interceptorServiceTypes = new List<Type>();
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

                // 注意：不能在BaseService类中使用[Transaction]，因为无效
                builder.RegisterType<TransactionInterceptor>(); // 配置事务拦截器
                interceptorServiceTypes.Add(typeof(TransactionInterceptor));

                builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerDependency();

                // 获取 Service 程序集服务，并注册，可以在实现方法中使用[Transaction]
                builder.RegisterAssemblyTypes(assemblys).InNamespace("ApiServer.BLL.BLL")
                    .Where(a => a.Name.EndsWith("Service"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope()
                    .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;动态代理
                    .InterceptedBy(interceptorServiceTypes.ToArray()); // 允许将拦截器服务的列表分配给注册。


                builder.RegisterGeneric(typeof(BaseDal<>)).As(typeof(IBaseDal<>)).InstancePerDependency();
                // 获取 Dal 程序集服务，并注册
                builder.RegisterAssemblyTypes(assemblys).InNamespace("ApiServer.DAL.DAL")
                    .Where(a => a.Name.EndsWith("Dal"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException);
            }
        }
    }
}
