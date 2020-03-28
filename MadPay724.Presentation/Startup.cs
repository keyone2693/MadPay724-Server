
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using MadPay724.Services.Seed.Service;
using MadPay724.Services.Site.Panel.Common.Service;
using MadPay724.Common.Routes.V1.Site;
using MadPay724.Presentation.Helpers.Configuration;

namespace MadPay724.Presentation
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedService seeder)
        {
            app.UseMadExceptionHandle(env);
            app.UseMadInitialize(seeder);
            app.UseMadAuth();
            app.UseMadSwagger();

            app.UseEndpoints(end =>
            {
                end.MapDefaultControllerRoute();
                end.MapHub<ChatHubService>(SiteV1Routes.BaseChatPanel + "/chat");
            });
        }
    }
}
