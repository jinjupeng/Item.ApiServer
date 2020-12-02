using Autofac;
using Item.ApiServer.DAL.DAL;
using Item.ApiServer.DAL.IDAL;

namespace Item.ApiServer.DAL.DALModule
{
    public class DalModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(BaseDal<>)).As(typeof(IBaseDal<>)).InstancePerDependency();
            //  builder.RegisterType<DapperHelper>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(this.ThisAssembly).InNamespace("Item.ApiServer.DAL.DAL")
                .Where(a => a.Name.EndsWith("DAL"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

        }
    }
}