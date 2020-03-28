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
                document.DocumentName = "v1_Api_";
                document.ApiGroupNames = new[] { "v1_Api_" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Users";
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
        }

        public static void UseMadSwagger(this IApplicationBuilder app)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3(); // serve Swagger UI
        }
   
    }
}
