using Autofac;
using AutoMapper;
using Charts.Shared.Api;
using Charts.Shared.Data.Primitives;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TTS.File.Api
{
    public class Startup : StartupShared
    {
        public Startup(IConfiguration configuration, IHostEnvironment hosting) : base(configuration, hosting)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            base.ConfigureServices(services, new[] { PortalEnum.Int.ToString(), PortalEnum.Ext.ToString() });

        }

        public new void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<AutofacModule>();
            base.ConfigureContainer(containerBuilder);
        }

        public new void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            base.Configure(app, loggerFactory, env);
        }
    }
}
