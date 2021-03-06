using Charts.Shared.Api;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Primitives;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Charts.Identity.Api
{
    public class Startup : StartupShared
    {
        public Startup(IConfiguration configuration, IHostEnvironment hosting) : base(configuration, hosting)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services, new[] { PortalEnum.Int.ToString(), PortalEnum.Ext.ToString() });
        }

        public new void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<AutofacModule>();
            base.ConfigureContainer(containerBuilder);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env, DataContext context)
        {
            base.Configure(app, loggerFactory, env);
            DataContextInitializer.SeedRolesAndPermissions(context);
            DataContextInitializer.SeedAdminUser(context, Configuration);
            DataContextInitializer.SeedDictionaries(context, Configuration);
        }
    }
}
