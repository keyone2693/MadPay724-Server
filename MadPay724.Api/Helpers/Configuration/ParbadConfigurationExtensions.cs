using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parbad.Builder;

namespace MadPay724.Api.Helpers.Configuration
{
    public static class ParbadConfigurationExtensions
    {
        public static void AddMadParbad(this IServiceCollection services, IConfiguration configuration)
        {
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
                        ef.UseSqlServer(@"Data Source=KEY1-LAB\MSSQLSERVER2016;Initial Catalog=Financial_MadPay724db;Integrated Security=True;MultipleActiveResultSets=True;",
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
