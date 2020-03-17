using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Auth.Interface;
using MadPay724.Services.Site.Panel.Auth.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
using Microsoft.AspNetCore.Identity;
using MadPay724.Common.OnlineChat.Storage;
using MadPay724.Data.DatabaseContext;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MadPay724.Common.Helpers.AppSetting;
using Microsoft.IdentityModel.Tokens;
using System;
using MadPay724.Presentation.Routes.V1;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace MadPay724.Presentation.Helpers.Configuration
{
    public static class IdentityConfigurationExtensions
    {
        public static void AddMadIdentityInit(this IServiceCollection services)
        {
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

        }
        public static void AddMadAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenSettingSection = configuration.GetSection("TokenSetting");
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
                            if (context.Request.Path.Value.StartsWith("/" + ApiV1Routes.BaseSitePanel + "/chat")
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

        public static void UseMadAuth(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");
            //
            app.UseAuthentication();
            app.UseAuthorization();

        }
    }
}
