
using Microsoft.Extensions.DependencyInjection;
using MadPay724.Data.Models.MainDB;
using Microsoft.AspNetCore.Identity;
using MadPay724.Data.DatabaseContext;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MadPay724.Common.Helpers.AppSetting;
using Microsoft.IdentityModel.Tokens;
using System;
using MadPay724.Common.Routes.V1.Site;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace MadPay724.Api.Helpers.Configuration
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
                });
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireNoAccess", policy => policy.RequireRole("NoAccess"));
                opt.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
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
