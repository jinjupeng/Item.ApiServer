using ApiServer.BLL.BLL;
using ApiServer.BLL.IBLL;
using Autofac;

namespace Item.ApiServer.BLL.BLLModule
{
    public class BllModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerDependency();
            //builder.RegisterType(typeof(CommonAuthorizeHandler)).InstancePerRequest();
            builder.RegisterAssemblyTypes(this.ThisAssembly).InNamespace("ApiServer.BLL.BLL")
                .Where(a => a.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

        }
    }
}