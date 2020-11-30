using System.Reflection;
using Autofac;

namespace TTS.File.Api
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.Load(new AssemblyName("TTS.File.Logic"));
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Logic"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterModule<Charts.Shared.Logic.AutofacModule>();
        }
    }
}
