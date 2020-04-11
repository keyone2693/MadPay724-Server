
using MadPay724.Api.Helpers.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Rewrite;
using MadPay724.Services.Seed.Service;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using NLog.Extensions.Logging;
using NLog.Web;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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



        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedService seeder)
        {
            app.UseMadExceptionHandle(env);
            app.UseMadInitialize(seeder);
            app.UseMadAuth();
            app.UseMadSwagger();
            app.UseMadParbad();

            //app.UseRewriter(new RewriteOptions().AddRedirect(@"^\s*$", "https://api.madpay724.ir/swagger", 301));


            app.UseEndpoints(end =>
            {
                end.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
                });
                end.MapControllers();
            });
        }
    }
}
