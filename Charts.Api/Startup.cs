using Autofac;
using Charts.Shared.Api;
using Charts.Shared.Data.Context;
using Charts.Shared.Data.Mongo.Context;
using Charts.Shared.Data.Primitives;
using Charts.Shared.Logic.Workflow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;

namespace Charts.Api
{
    public class Startup : StartupShared
    {
        public Startup(IConfiguration configuration, IHostEnvironment hosting) : base(configuration, hosting)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services, new[] { PortalEnum.Int.ToString(), PortalEnum.Ext.ToString() });
            services.AddWorkflow(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),true,true));
        }

        public new void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<AutofacModule>();
            base.ConfigureContainer(containerBuilder);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env, DataContext context)
        {
           
            base.Configure(app, loggerFactory, env);
            var host = app.ApplicationServices.GetService<IWorkflowHost>();
            host.RegisterWorkflow<ApplicationWorkflow>();
            host.Start();
            host.StartWorkflow("TTSWorkflow", 1, null);
        }
    }
}
