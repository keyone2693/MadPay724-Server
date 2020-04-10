using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parbad.Builder;

namespace MadPay724.Payment.Helpers.Configuration
{
    public static class ParbadConfigurationExtensions
    {
        public static void AddMadParbad(this IServiceCollection services, IConfiguration configuration)
        {
            var con = configuration.GetSection("ConnectionStrings");

            services.AddParbad()
                .ConfigureGateways(gateWayes =>
                {
                    gateWayes
                    .AddMellat()
                    .WithAccounts(accs =>
                    {
                        accs.AddFromConfiguration(configuration.GetSection("MellatBank"));
                    });
                    gateWayes
                    .AddZarinPal()
                    .WithAccounts(accs =>
                    {
                        accs.AddFromConfiguration(configuration.GetSection("ZarinPalBank"));
                    });
                    gateWayes
                    .AddParbadVirtual()
                    .WithOptions(bld => bld.GatewayPath = "/MadpayGateWay");
                })
                .ConfigureHttpContext(bld => bld.UseDefaultAspNetCore())
                .ConfigureStorage(bld =>
                {
                    bld.UseEntityFrameworkCore(ef =>
                        ef.UseSqlServer(con.GetSection("Financial").Value,
                        opt => opt.UseParbadMigrations()))
                    .ConfigureDatabaseInitializer(bld =>
                    {
                        bld.CreateAndMigrateDatabase();
                    });
                });
        }

        public static void UseMadParbad(this IApplicationBuilder app)
        {
            app.UseParbadVirtualGateway();
        }
    }
}
