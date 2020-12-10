using ApiServer.DAL.DAL;
using ApiServer.DAL.IDAL;
using Autofac;

namespace Item.ApiServer.DAL.DALModule
{
    public class DalModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(BaseDal<>)).As(typeof(IBaseDal<>)).InstancePerDependency();
            //  builder.RegisterType<DapperHelper>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(this.ThisAssembly).InNamespace("ApiServer.DAL.DAL")
                .Where(a => a.Name.EndsWith("Dal"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

        }
    }
}