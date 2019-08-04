using System;
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
using AutoMapper;
using MadPay724.Common.Helpers.AppSetting;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.MediaTypes;
using MadPay724.Data.Models;
using MadPay724.Services.Upload.Interface;
using MadPay724.Services.Upload.Service;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Services.Seed.Interface;
using MadPay724.Services.Seed.Service;
using MadPay724.Services.Site.Admin.User.Interface;
using MadPay724.Services.Site.Admin.User.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

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

            services.AddDbContext<MadpayDbContext>(p => p.UseSqlServer(
                @"Data Source=KEY1-LAB\MSSQLSERVER2016;Initial Catalog=MadPay724db;Integrated Security=True;MultipleActiveResultSets=True;"));

            services.AddMvc(config =>
                {
                    config.EnableEndpointRouting = false;
                    config.ReturnHttpNotAcceptable = true;
                    config.SslPort = _httpsPort;
                    config.Filters.Add(typeof(RequireHttpsAttribute));
                    //config.Filters.Add(typeof(LinkRewritingFilter));
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
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



            //services.AddAuthentication("Bearer")
            //    .AddIdentityServerAuthentication(opt =>
            //    {
            //        opt.Authority = "http://localhost:5000";
            //        opt.RequireHttpsMetadata = false;
            //        opt.ApiName = "MadPay724Api";
            //    });

            services.AddResponseCaching();
            services.AddHsts(opt =>
            {
                opt.MaxAge = TimeSpan.FromDays(180);
                opt.IncludeSubDomains = true;
                opt.Preload = true;
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
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel";
                document.ApiGroupNames = new[] { "v1_Site_Panel" };
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

            services.AddCors();


            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUnitOfWork<MadpayDbContext>, UnitOfWork<MadpayDbContext>>();
            services.AddTransient<SeedService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IUtilities, Utilities>();
            services.AddScoped<UserCheckIdFilter>();
            //services.AddScoped<TokenSetting>();

            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });
            builder = new IdentityBuilder(builder.UserType,typeof(Role),builder.Services);
            builder.AddEntityFrameworkStores<MadpayDbContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            builder.AddDefaultTokenProviders();
           
            //services.Configure<TokenSetting>(Configuration.GetSection("TokenSetting"));

            var tokenSettingSection = Configuration.GetSection("TokenSetting");
            var tokenSetting= tokenSettingSection.Get<TokenSetting>();
            var key = Encoding.ASCII.GetBytes(tokenSetting.Secret);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = tokenSetting.Site,
                        ValidateAudience = true,
                        ValidAudience = tokenSetting.Audience,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));

                opt.AddPolicy("AccessBlog", policy => policy.RequireRole("Admin", "Blog"));
                opt.AddPolicy("AccessAccounting", policy => policy.RequireRole("Admin", "Accountant"));


                opt.AddPolicy("AccessProfile", policy => policy.RequireRole("Admin", "User", "Blog", "Accountant"));

                opt.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
                opt.AddPolicy("RequireBlogsRole", policy => policy.RequireRole("Blog"));
                opt.AddPolicy("RequireAccountantRole", policy => policy.RequireRole("Accountant"));




            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedService seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
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
                app.UseHsts();
                app.UseHttpsRedirection();
                app.UseResponseCaching();
            }

           
            //app.UseResponseCompression();
            
            seeder.SeedUsers();
            app.UseCors(p => 
                p.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            //
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
