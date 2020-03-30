using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Linq;

namespace MadPay724.Api.Helpers.Configuration
{
    public static class SwaggerConfigurationExtensions
    {
        public static void AddMadSwagger(this IServiceCollection services)
        {
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Api_Pay";
                document.ApiGroupNames = new[] { "v1_Api_Pay" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Payment Section";
                };
            });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Api_Verify";
                document.ApiGroupNames = new[] { "v1_Api_Verify" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Verify Section";
                };
            });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Api_Refund";
                document.ApiGroupNames = new[] { "v1_Api_Refund" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Refund Section";
                };
            });
        }

        public static void UseMadSwagger(this IApplicationBuilder app)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3(); // serve Swagger UI
        }
   
    }
}
