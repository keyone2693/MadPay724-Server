﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parbad.Builder;

namespace MadPay724.Api.Helpers.Configuration
{
    public static class ParbadConfigurationExtensions
    {
        public static void AddMadParbad(this IServiceCollection services)
        {
            services.AddParbad()
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
        }
    }
}
