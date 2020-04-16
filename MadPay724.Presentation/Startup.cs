
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
using Syncfusion.Licensing;

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
            services.AddMadDbContext(Configuration);
            services.AddMadInitialize(_httpsPort);
            services.AddSignalR();
            services.AddAutoMapper(typeof(Startup));
            services.AddMadDI();
            services.AddMadIdentityInit();
            services.AddMadAuth(Configuration);
            services.AddMadApiVersioning();
            services.AddMadSwagger();
            services.AddMadParbad(Configuration);
            services.AddMadSpa();

            //services.AddRendertron(options =>
            //{
            //    options.RendertronUrl = "https://render-tron.appspot.com/render/";

            //    options.UserAgents.Add("firefox");
            //    options.UserAgents.Add("googlebot");
            //    options.UserAgents.Add("yandexbot");
            //    options.UserAgents.Add("duckduckbot");
            //    options.UserAgents.Add("slurp");
            //    options.UserAgents.Add("Baiduspider");
            //    options.UserAgents.Add("bingbot");
            //    options.UserAgents.Add("Embedly");
            //    options.UserAgents.Add("facebookexternalhit");
            //    options.UserAgents.Add("LinkedInBot");
            //    options.UserAgents.Add("outbrain");
            //    options.UserAgents.Add("pinterest");
            //    options.UserAgents.Add("quora link preview");
            //    options.UserAgents.Add("rogerbot");
            //    options.UserAgents.Add("showyoubot");
            //    options.UserAgents.Add("Slackbot");
            //    options.UserAgents.Add("TelegramBot");
            //    options.UserAgents.Add("Twitterbot");
            //    options.UserAgents.Add("vkShare");
            //    options.UserAgents.Add("W3C_Validator");
            //    options.UserAgents.Add("WhatsApp");
            //});
        }

        [System.Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedService seeder)
        {
            SyncfusionLicenseProvider
                .RegisterLicense(Configuration.GetSection("TokenSetting")
                .GetSection("SyncfusionKey").Value);

            app.UseMadExceptionHandle(env);

            app.UseMadInitialize(seeder);
            app.UseMadAuth();
            app.UseMadSwagger();
            app.UseMadParbad();

            //app.UseRendertron();

            app.UseEndpoints(end =>
            {
                end.MapControllers();
                end.MapControllerRoute(
                     name: "default",
                   pattern: "{controller=home}/{action=index}");
                end.MapFallbackToController("Index", "FallBack");
                end.MapHub<ChatHubService>(SiteV1Routes.BaseChatPanel + "/chat");

            });

            app.UseMadSpa();
        }

    }
    
}
