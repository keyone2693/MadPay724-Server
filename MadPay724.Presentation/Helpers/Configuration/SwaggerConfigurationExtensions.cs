using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;
using System.Linq;

namespace MadPay724.Presentation.Helpers.Configuration
{
    public static class SwaggerConfigurationExtensions
    {
        public static void AddMadSwagger(this IServiceCollection services)
        {
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Layout";
                document.ApiGroupNames = new[] { "v1_Site_Layout" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Site Layout";
                };
            });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Home";
                document.ApiGroupNames = new[] { "v1_Site_Home" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Site";
                };
            });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Blog";
                document.ApiGroupNames = new[] { "v1_Site_Blog" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Site";
                };
            });
            //
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel";
                document.ApiGroupNames = new[] { "v1_Site_Panel" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Users";
                    //d.Info.Contact = new OpenApiContact
                    //{
                    //    Name = "keyone",
                    //    Email = string.Empty,
                    //    Url = "https://twitter.com/keyone"
                    //};
                    //d.Info.License = new OpenApiLicense
                    //{
                    //    Name = "Use under LICX",
                    //    Url = "https://example.com/license"
                    //};
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
                //      new OperationSecurityScopeProcessor("JWT"));
            });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel_Blog";
                document.ApiGroupNames = new[] { "v1_Site_Panel_Blog" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Blog , AdminBlog , Admin";
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
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel_Common";
                document.ApiGroupNames = new[] { "v1_Site_Panel_Common" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Accountant , AdminBlog , Admin";
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
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel_Accountant";
                document.ApiGroupNames = new[] { "v1_Site_Panel_Accountant" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Accountant , Admin";
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
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v1_Site_Panel_Admin";
                document.ApiGroupNames = new[] { "v1_Site_Panel_Admin" };
                document.PostProcess = d =>
                {
                    d.Info.Title = "MadPay724 Api Docs For Admin";
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
