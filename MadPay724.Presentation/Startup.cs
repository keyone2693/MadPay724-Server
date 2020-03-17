using System;
using System.Linq;
using MadPay724.Data.DatabaseContext;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Auth.Interface;
using MadPay724.Services.Site.Panel.Auth.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using NSwag;
using NSwag.Generation.Processors.Security;
using AutoMapper;
using MadPay724.Common.Helpers.AppSetting;
using MadPay724.Common.Helpers.Utilities;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.Models.MainDB;
using MadPay724.Services.Upload.Interface;
using MadPay724.Services.Upload.Service;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Services.Seed.Service;
using MadPay724.Services.Site.Panel.User.Interface;
using MadPay724.Services.Site.Panel.User.Service;
using MadPay724.Services.Site.Panel.Wallet.Interface;
using MadPay724.Services.Site.Panel.Wallet.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using MadPay724.Common.Helpers.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using MadPay724.Services.Site.Panel.Common.Service;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Common.OnlineChat.Storage;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using MadPay724.Data.Dtos.Common;
using Newtonsoft.Json;
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
                end.MapHub<ChatHubService>(ApiV1Routes.BaseSitePanel + "/chat");
            });
        }
    }
}
