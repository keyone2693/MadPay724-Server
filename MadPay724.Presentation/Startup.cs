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
            services.AddDbContext<Main_MadPayDbContext>();
            services.AddDbContext<Financial_MadPayDbContext>();
            services.AddDbContext<Log_MadPayDbContext>();

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
                    d.Info.Title = "MadPay724 Api Docs For Users";
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

            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel_Blog";
                document.ApiGroupNames = new[] { "v1_Site_Panel_Blog" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Blog , AdminBlog , Admin";
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
            });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel_Common";
                document.ApiGroupNames = new[] { "v1_Site_Panel_Common" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Accountant , AdminBlog , Admin";
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
            });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel_Accountant";
                document.ApiGroupNames = new[] { "v1_Site_Panel_Accountant" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Accountant , Admin";
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
            });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel_Admin";
                document.ApiGroupNames = new[] { "v1_Site_Panel_Admin" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Admin";
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
            });
            services.AddCors(opt =>
            opt.AddPolicy("CorsPolicy", builder =>
           builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials()));

            services.AddSignalR();
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<UserInfoInMemory>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));


            services.AddTransient<SeedService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IUtilities, Utilities>();
            services.AddScoped<UserCheckIdFilter>();
            services.AddScoped<IsBloggerHimselfFilter>();
            services.AddScoped<DocumentApproveFilter>();
            //services.AddScoped<TokenSetting>();

            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<Main_MadPayDbContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();
            builder.AddUserManager<UserManager<User>>();
            builder.AddDefaultTokenProviders();

            //services.Configure<TokenSetting>(Configuration.GetSection("TokenSetting"));

            var tokenSettingSection = Configuration.GetSection("TokenSetting");
            var tokenSetting = tokenSettingSection.Get<TokenSetting>();
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
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Path.Value.StartsWith("chat")
                            && context.Request.Query.TryGetValue("access_token", out StringValues token))
                            {
                                context.Token = token;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireNoAccess", policy => policy.RequireRole("NoAccess"));
                opt.AddPolicy("AccessChat", policy => policy.RequireRole("Admin", "User"));

                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));

                opt.AddPolicy("AccessBlog", policy => policy.RequireRole("Admin", "Blog", "AdminBlog"));

                opt.AddPolicy("AccessBloger", policy => policy.RequireRole("Blog", "AdminBlog"));

                opt.AddPolicy("AccessAdminBlog", policy => policy.RequireRole("Admin", "AdminBlog"));
                opt.AddPolicy("AccessAccounting", policy => policy.RequireRole("Admin", "Accountant"));

                opt.AddPolicy("AccessNotify", policy => policy.RequireRole("Admin", "Accountant", "AdminBlog", "User"));

                opt.AddPolicy("AccessProfile", policy => policy.RequireRole("Admin", "User", "AdminBlog", "Blog", "Accountant"));

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
            app.UseCors("CorsPolicy");
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


            //app.UseMvc();

            //app.UseSignalR()

            app.UseEndpoints(end =>
            {
                end.MapDefaultControllerRoute();
                end.MapHub<ChatHubService>(ApiV1Routes.BaseSitePanel + "/chat");
            });
        }
    }
}
