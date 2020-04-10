using ImageResizer.AspNetCore.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Api;
using MadPay724.Services.Seed.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MadPay724.Api.Helpers.Configuration
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
                builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
             })
             .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            //Custom ModelState Error
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.InvalidModelStateResponseFactory = context =>
                {
                    var strErrorList = new List<string>();
                    var msErrors = context.ModelState.Where(p => p.Value.Errors.Count > 0);
                    foreach (var msError in msErrors)
                    {
                        foreach (var error in msError.Value.Errors)
                        {
                            strErrorList.Add(error.ErrorMessage);
                        }
                    }
                    var errorModel = new GateApiReturn<string>
                    {
                        Status = false,
                        Messages = strErrorList.ToArray(),
                        Result = null
                    };
                    return new BadRequestObjectResult(errorModel);
                };
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
            //services.AddResponseCompression(opt => opt.Providers.Add<GzipCompressionProvider>());
            //services.AddRouting( opt => opt.LowercaseUrls = true);
            //services.AddApiVersioning(opt =>
            //{
            //    opt.ApiVersionReader = new MediaTypeApiVersionReader();
            //    opt.AssumeDefaultVersionWhenUnspecified = true;
            //    opt.ReportApiVersions = true;
            //    opt.DefaultApiVersion = new ApiVersion(1,0);
            //    opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            //});
            services.AddImageResizer();
        }

        public static void UseMadInitialize(this IApplicationBuilder app)
        {
            //app.UseResponseCompression();
            app.UseRouting();
            app.UseImageResizer();
            app.UseCsp(opt => opt.DefaultSources(s => s.Self())
            .StyleSources(s => s.Self().UnsafeInline())
            .ScriptSources(s => s.Self().UnsafeInline())
            .ImageSources(s => s.Self().CustomSources("res.cloudinary.com", "cloudinary.com", "data:"))
            .MediaSources(s => s.Self().CustomSources("res.cloudinary.com", "cloudinary.com", "data:"))
            .FontSources(s => s.Self().CustomSources("data:"))
            );
            app.UseXfo(o => o.Deny());
            app.UseStaticFiles();
        }

        public static void UseMadInitializeInProd(this IApplicationBuilder app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseResponseCaching();
        }
    }
}
