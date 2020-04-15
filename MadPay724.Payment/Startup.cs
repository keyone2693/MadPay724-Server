using AutoMapper;
using MadPay724.Payment.Helpers.Configuration;
using MadPay724.Payment.Helpers.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MadPay724.Payment
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddRazorPages();
            services.AddMadDbContext(Configuration);
            services.AddMadInitialize();
            services.AddAutoMapper(typeof(Startup));
            services.AddMadDI();

            services.AddMadParbad(Configuration);

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMadExceptionHandle(env);
            app.UseMadInitialize();
            app.UseMadParbad();

            var rewriteOptions = new RewriteOptions();
            rewriteOptions.Rules.Add(new NonWwwRewriteRule());
            rewriteOptions.AddRedirect(@"^\s*$", "https://madpay724.ir", 301);

            app.UseRewriter(rewriteOptions);


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                      name: "pay",
                    pattern: "{controller=bank}/{action=pay}/{token?}");
            });
        }
    }
}
