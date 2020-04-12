
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;


namespace MadPay724.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //logging.AddNLog();
                    logging.AddEntityFramework<Log_MadPayDbContext, ExtendedLog>();
                });
    }
}
