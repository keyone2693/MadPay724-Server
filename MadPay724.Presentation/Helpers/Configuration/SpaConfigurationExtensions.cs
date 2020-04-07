using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Helpers.Configuration
{
    public static class SpaConfigurationExtensions
    {
        public static void AddMadSpa(this IServiceCollection services)
        {

        }

        public static void UseMadSpa(this IApplicationBuilder app)
        {
            app.UseSpa(spa =>
           {
             
           });
        }
    }
}
