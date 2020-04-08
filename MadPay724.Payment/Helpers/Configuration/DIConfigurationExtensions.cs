using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.Utilities;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Common;
using MadPay724.Services.Site.Panel.Wallet.Interface;
using MadPay724.Services.Site.Panel.Wallet.Service;
using Microsoft.Extensions.DependencyInjection;
using DnsClient;
using Microsoft.AspNetCore.Http;

namespace MadPay724.Payment.Helpers.Configuration
{
    public static class DIConfigurationExtensions
    {
        public static void AddMadDI(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //
            // services.AddScoped<IWalletService, WalletService>();
            // services.AddScoped<IUtilities, Utilities>();
            services.AddScoped<ISmsService, SmsService>();
            //
            services.AddSingleton<ILookupClient, LookupClient>();

        }
    }
}
