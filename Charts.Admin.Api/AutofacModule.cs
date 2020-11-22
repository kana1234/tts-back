using Autofac;
using System.Reflection;

namespace Charts.Admin.Api
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.Load(new AssemblyName("Charts.Admin.Logic"));
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Logic"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterModule<Shared.Logic.AutofacModule>();
        }
    }
}
