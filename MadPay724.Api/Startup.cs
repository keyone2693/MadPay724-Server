
using MadPay724.Api.Helpers.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MadPay724.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly int? _httpsPort;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            if (env.IsDevelopment())
            {
                var lunchJsonConf = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("Properties\\launchSettings.json")
                    .Build();
                _httpsPort = lunchJsonConf.GetValue<int>("iisSettings:iisExpress:sslPort");
            }
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMadDbContext();
            services.AddMadInitialize(_httpsPort);
            services.AddSignalR();
            services.AddAutoMapper(typeof(Startup));
            services.AddMadDI();
            services.AddMadIdentityInit();
            services.AddMadAuth(Configuration);
            services.AddMadApiVersioning();
            services.AddMadSwagger();



        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMadExceptionHandle(env);
            app.UseMadInitialize();
            app.UseMadAuth();
            app.UseMadSwagger();

            app.UseEndpoints(end =>
            {
                end.MapDefaultControllerRoute();
            });
        }
    }
}
