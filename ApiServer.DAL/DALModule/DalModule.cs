using ApiServer.DAL.DAL;
using ApiServer.DAL.IDAL;
using Autofac;
using System;

namespace Item.ApiServer.DAL.DALModule
{
    public class DalModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                builder.RegisterGeneric(typeof(BaseDal<>)).As(typeof(IBaseDal<>)).InstancePerDependency();
                builder.RegisterAssemblyTypes(this.ThisAssembly).InNamespace("ApiServer.DAL.DAL")
                    .Where(a => a.Name.EndsWith("Dal"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\n" + ex.InnerException);
            }
        }
    }
}