using System.Linq;
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
using MadPay724.Services.Seed.Service;
using MadPay724.Services.Seed.Interface;
using AutoMapper;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.MediaTypes;
using MadPay724.Services.Upload.Interface;
using MadPay724.Services.Upload.Service;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Services.Site.Admin.User.Interface;
using MadPay724.Services.Site.Admin.User.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace MadPay724.Presentation
{
    public class Startup
    {
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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config=>
                {
                    config.EnableEndpointRouting = false;
                    config.ReturnHttpNotAcceptable = true;
                    config.SslPort = _httpsPort;
                    config.Filters.Add(typeof(RequireHttpsAttribute));

                    //var jsonFormatter = config.OutputFormatters.OfType<JsonOutputFormatter>().Single();
                    //config.OutputFormatters.Remove(jsonFormatter);
                    //config.OutputFormatters.Add(new IonOutputFormatter(jsonFormatter));
                    //config.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    //config.InputFormatters.Add(new XmlSerializerInputFormatter(config));
                })
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            //services.AddRouting( opt => opt.LowercaseUrls = true);
            //services.AddApiVersioning(opt =>
            //{
            //    opt.ApiVersionReader = new MediaTypeApiVersionReader();
            //    opt.AssumeDefaultVersionWhenUnspecified = true;
            //    opt.ReportApiVersions = true;
            //    opt.DefaultApiVersion = new ApiVersion(1,0);
            //    opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            //});
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Admin";
                document.ApiGroupNames = new[] { "v1_Site_Admin" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs";
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

            app.UseHttpsRedirection();
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
