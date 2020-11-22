using Autofac;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Charts.Shared.Data
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.Load(new AssemblyName("Charts.Shared.Data"));
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Repo"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
     
        }
    }
}
