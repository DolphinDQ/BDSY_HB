using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirStandard.Core.Data;
using AirStandard.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AirStandard.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IServiceProvider ServiceProvider { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            #region Dependency
            services.AddSingleton<IDataManager, LearnCloudDataManager>();

            #endregion


            #region Options
            services.AddOptions();
            services.Configure<LearnCloudOptions>(Configuration.GetSection("LearnCloud"));

            #endregion
            ServiceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(router =>
            {
                router.MapRoute("default", "api/{controller}/{action}/{id?}");
            });
            lifetime.ApplicationStarted.Register(OnStarted);
            lifetime.ApplicationStopped.Register(OnStopped);
        }

        private void OnStopped()
        {
            ServiceProvider.GetService<IDataManager>().Dispose();
        }

        private void OnStarted()
        {
            ServiceProvider.GetService<IDataManager>().Init();
        }
    }
}
