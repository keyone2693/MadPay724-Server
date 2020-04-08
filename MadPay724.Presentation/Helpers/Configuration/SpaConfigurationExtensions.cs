using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace MadPay724.Presentation.Helpers.Configuration
{
    public static class SpaConfigurationExtensions
    {
        public static void AddMadSpa(this IServiceCollection services)
        {
            services.AddSpaStaticFiles(conf =>
            {
                conf.RootPath = "Clients";
            });
        }

        public static void UseMadSpa(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = new PathString("/wwwroot")
            });

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            app.UseDefaultFiles();
            app.UseSpaStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
            //
            app.UseRewriter(new RewriteOptions().AddRewrite(@"^\s*$", "/app", skipRemainingRules: true));
            app.Map("/app", site =>
                {
                    site.UseSpa(spa =>
                    {
                        spa.Options.SourcePath = "Clients/app";
                        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                        {
                            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Clients/app"))
                        };
                        //spa.UseSpaPrerendering(options =>
                        //{
                        //    options.BootModulePath = $"{spa.Options.SourcePath}/dist-server/main.js";
                        //    options.ExcludeUrls = new[] { "/sockjs-node" };
                        //});
                    });
                }).Map("/my", panel =>
                {
                    panel.UseSpa(spa =>
                    {
                        spa.Options.SourcePath = "Clients/my";
                        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                        {
                            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Clients/my"))
                        };
                    });
                });
        }
    }
}
