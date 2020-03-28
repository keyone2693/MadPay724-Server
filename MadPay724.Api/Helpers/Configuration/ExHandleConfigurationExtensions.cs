using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.Dtos.Api;
using MadPay724.Data.Dtos.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Net;

namespace MadPay724.Api.Helpers.Configuration
{
    public static class ExHandleConfigurationExtensions
    {
        public static void UseMadExceptionHandle(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            var model = JsonConvert.SerializeObject(new GateApiReturn<string>
                            {
                                Status = false,
                                Message = new string[] {error.Error.Message},
                                Result = null
                            });
                            context.Response.AddAppError(model);
                            await context.Response.WriteAsync(model);
                        }
                    });
                });
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseMadInitializeInProd();
            }
        }
    }
}
