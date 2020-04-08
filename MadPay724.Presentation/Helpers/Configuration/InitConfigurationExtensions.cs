using ImageResizer.AspNetCore.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Services.Seed.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MadPay724.Presentation.Helpers.Configuration
{
    public static class InitConfigurationExtensions
    {
        public static void AddMadDbContext(this IServiceCollection services)
        {
            services.AddDbContext<Main_MadPayDbContext>();
            services.AddDbContext<Financial_MadPayDbContext>();
            services.AddDbContext<Log_MadPayDbContext>();
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
                builder.WithOrigins("http://localhost:4200", "http://127.0.0.1:8080")
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
                opt.RedirectStatusCode = StatusCodes.Status302Found;
            });
            //
            services.AddResponseCompression(opt => opt.Providers.Add<GzipCompressionProvider>());
            //services.AddRouting( opt => opt.LowercaseUrls = true);
            //
            services.AddImageResizer();
        }

        public static void UseMadInitialize(this IApplicationBuilder app, SeedService seeder)
        {
            //app.UseResponseCompression();
            seeder.SeedUsers();
            app.UseRouting();
            app.UseImageResizer();

            app.UseCsp(opt => opt
            .StyleSources(s=>s.Self().UnsafeInline().CustomSources("fonts.googleapis.com"))
            .ScriptSources(s=>s.Self().UnsafeInline().CustomSources("apis.google.com", "connect.facebook.net"))
            .FontSources(s => s.Self().CustomSources("fonts.gstatic.com", "data:"))
            .FrameSources(s=>s.Self().CustomSources("accounts.google.com"))
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
