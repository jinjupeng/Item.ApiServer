using ApiServer.BLL.BLL;
using ApiServer.BLL.IBLL;
using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;

namespace Item.ApiServer.BLL.BLLModule
{
    public class BllModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                var interceptorServiceTypes = new List<Type>();
                builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerDependency();
                builder.RegisterAssemblyTypes(this.ThisAssembly).InNamespace("ApiServer.BLL.BLL")
                    .Where(a => a.Name.EndsWith("Service"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope()
                    .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                    .InterceptedBy(interceptorServiceTypes.ToArray()); // 允许将拦截器服务的列表分配给注册。
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException);
            }
        }
    }
}