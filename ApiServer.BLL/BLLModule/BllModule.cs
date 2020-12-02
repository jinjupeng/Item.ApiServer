using Autofac;
using Item.ApiServer.BLL.BLL;
using Item.ApiServer.BLL.IBLL;

namespace Item.ApiServer.BLL.BLLModule
{
    public class BllModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>)).InstancePerDependency();
            //builder.RegisterType(typeof(CommonAuthorizeHandler)).InstancePerRequest();
            builder.RegisterAssemblyTypes(this.ThisAssembly).InNamespace("Item.ApiServer.BLL.BLL")
                .Where(a => a.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

        }
    }
}