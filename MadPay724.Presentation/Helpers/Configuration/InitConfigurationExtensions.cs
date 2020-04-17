using ImageResizer.AspNetCore.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Services.Seed.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MadPay724.Presentation.Helpers.Configuration
{
    public static class InitConfigurationExtensions
    {
        public static void AddMadDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var con = configuration.GetSection("ConnectionStrings");

            services.AddDbContext<Main_MadPayDbContext>(opt => { 
                opt.UseSqlServer(con.GetSection("Main").Value);
            });
            services.AddDbContext<Financial_MadPayDbContext>(opt => {
                opt.UseSqlServer(con.GetSection("Financial").Value);
            });
            services.AddDbContext<Log_MadPayDbContext>(opt => {
                opt.UseSqlServer(con.GetSection("Log").Value);
            });
        }
        public static void AddMadInitialize(this IServiceCollection services, int? httpsPort)
        {
            services.AddControllersWithViews();
            services.AddMvcCore(config =>
            {
                config.ReturnHttpNotAcceptable = true;
                config.Filters.Add(typeof(RequireHttpsAttribute));
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
             .AddApiExplorer()
             .AddFormatterMappings()
             .AddDataAnnotations()
             .AddCors(opt =>
             {
                opt.AddPolicy("CorsPolicy", builder =>
                builder.WithOrigins("https://madpay724.ir", "https://api.madpay724.ir", "https://pay.madpay724.ir")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
             })
             .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            //
            services.AddResponseCaching();
            services.AddHsts(opt =>
            {
                opt.MaxAge = TimeSpan.FromDays(180);
                opt.IncludeSubDomains = true;
                opt.Preload = true;
            });

            services.AddHttpsRedirection(opt =>
            {
                opt.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            });
            //
            services.AddResponseCompression(opt => opt.Providers.Add<GzipCompressionProvider>());
            //services.AddRouting( opt => opt.LowercaseUrls = true);
            //
            services.AddImageResizer();
        }

        public static void UseMadInitialize(this IApplicationBuilder app, SeedService seeder)
        {
            app.UseResponseCompression();
            seeder.SeedUsers();
            app.UseRouting();
            app.UseImageResizer();

            app.UseCsp(opt => opt
            .StyleSources(s=>s.Self()
            .UnsafeInline().CustomSources("pay.madpay724.ir", "api.madpay724.ir","fonts.googleapis.com"))
            .ScriptSources(s=>s.Self()
            .UnsafeInline().UnsafeEval().CustomSources("pay.madpay724.ir", "api.madpay724.ir","apis.google.com", "connect.facebook.net"))
            .ImageSources(s => s.Self()
            .CustomSources("pay.madpay724.ir", "api.madpay724.ir", "res.cloudinary.com", "cloudinary.com", "data:"))
            .MediaSources(s => s.Self()
            .CustomSources("pay.madpay724.ir", "api.madpay724.ir", "res.cloudinary.com", "cloudinary.com", "data:"))
            .FontSources(s => s.Self()
            .CustomSources("fonts.gstatic.com", "data:"))
            .FrameSources(s=>s.Self()
            .CustomSources("accounts.google.com"))
            );


            app.UseXfo(o => o.Deny());

           

        }

        public static void UseMadInitializeInProd(this IApplicationBuilder app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseResponseCaching();
        }
    }
}
