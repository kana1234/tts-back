using Charts.Shared.Data;
using Charts.Shared.Data.Context;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Charts.Shared.Api
{
    /// <summary>
    /// Базовый startup
    /// </summary>
    public class StartupShared
    {
        public StartupShared(IConfiguration configuration, IHostEnvironment env)
        {
            Environment = env;
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddJsonFile("appsettings.shared.json")
                .AddJsonFile($"appsettings.shared.{env.EnvironmentName.Trim()}.json")
                .AddConfiguration(configuration);
            Configuration = configurationBuilder.Build();
            if (Configuration["Serilog:WriteTo:2:Args:pathFormat"] != null)
                Configuration["Serilog:WriteTo:2:Args:pathFormat"] =
                    Configuration["Serilog:WriteTo:2:Args:pathFormat"].Replace("{Application}", env.ApplicationName);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
        }
        protected IConfiguration Configuration { get; }
        protected IHostEnvironment Environment { get; set; }

        protected void ConfigureContainer(ContainerBuilder builder)
        {
        }

        protected void ConfigureServices(IServiceCollection services, string[] authScopes = null)
        { 
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
            //services.Add(new ServiceDescriptor(typeof(DataContext), new DataContext(Configuration.GetConnectionString("DefaultConnection"))));
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddCors();
            services.AddControllers()
                .AddNewtonsoftJson(opt =>
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddOptions();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "You api title", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = Configuration["AppSettings:AuthOptions:Issuer"],

                            ValidateAudience = true,
                            ValidAudiences = authScopes.ToArray() ?? null,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,

                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AppSettings:AuthOptions:Key"])),
                            ValidateIssuerSigningKey = true
                        };
                        
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                {
                                    context.Response.Headers.Add("X-Token-Expired", "true");
                                    context.Response.Headers.Add("Access-Control-Expose-Headers", "X-Token-Expired");
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });
            services.AddControllersWithViews();
            services.AddResponseCompression();
        }

        protected void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "My API");
            });
            loggerFactory.AddSerilog();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
