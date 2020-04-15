using MadPay724.Data.Models;
using MadPay724.Data.Models.MainDB;
using Microsoft.EntityFrameworkCore;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace MadPay724.Data.DatabaseContext
{
    public class Log_MadPayDbContext : DbContext
    {
        public Log_MadPayDbContext()
        {

        }
        public Log_MadPayDbContext(DbContextOptions<Log_MadPayDbContext> opt) : base(opt)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {

            optionBuilder
                .UseSqlServer(
             @"Server=.\MSSQLSERVER2017;Initial Catalog=Log_MadPay724db;User Id=keyvan_madpay724;Password=3?55Tilp;"
            //"Server=KEY1-LAB\\MSSQLSERVER2016;Database=Log_MadPay724db;User Id=sa;Password=sa;Trusted_Connection=True;MultipleActiveResultSets=True;"
            );
        }

        public DbSet<ExtendedLog> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            LogModelBuilderHelper.Build(modelBuilder.Entity<ExtendedLog>());

            modelBuilder.Entity<ExtendedLog>().ToTable("ExtendedLog");
        }
    }
}
