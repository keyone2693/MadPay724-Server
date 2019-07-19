
using System.Collections.Generic;
using System.Linq;
using MadPay724.Common.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using MadPay724.Services.Site.Admin.Auth.Service;
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
using Microsoft.AspNetCore.Mvc.Versioning;
using MadPay724.Services.Seed.Service;
using MadPay724.Services.Seed.Interface;
using AutoMapper;
using System.IO;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Common.Helpers.Interface;
using Microsoft.Extensions.FileProviders;
using MadPay724.Services.Upload.Interface;
using MadPay724.Services.Upload.Service;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Services.Site.Admin.User.Interface;
using MadPay724.Services.Site.Admin.User.Service;

namespace MadPay724.Presentation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt=>opt.EnableEndpointRouting = false)
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "Site";
                document.ApiGroupNames = new[] { "Site", "Users" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "Hello world!";
                    //d.Info.Contact = new OpenApiContact
                    //{
                    //    Name = "keyone",
                    //    Email = string.Empty,
                    //    Url = "https://twitter.com/keyone"
                    //};
                    //d.Info.License = new OpenApiLicense
                    //{
                    //    Name = "Use under LICX",
                    //    Url = "https://example.com/license"
                    //};
                };


                document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                //      new OperationSecurityScopeProcessor("JWT"));


            });
            //services.AddOpenApiDocument(document =>
            //{
            //    document.DocumentName = "Api";
            //    document.ApiGroupNames = new[] { "Api" };
            //});


            services.AddCors();

            //services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IUnitOfWork<MadpayDbContext> , UnitOfWork<MadpayDbContext>>();
            services.AddTransient<ISeedService, SeedService>();
            services.AddScoped<IAuthService , AuthService>();
            services.AddScoped<IUserService , UserService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IUtilities, Utilities>();
            services.AddScoped<UserCheckIdFilter>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

      


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISeedService seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddAppError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            //seeder.SeedUsers();
            app.UseCors(p => p.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());
            ///
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseOpenApi();
            app.UseSwaggerUi3(); // serve Swagger UI

            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = new PathString("/wwwroot")
            });


            app.UseMvc();
        }
    }
}
