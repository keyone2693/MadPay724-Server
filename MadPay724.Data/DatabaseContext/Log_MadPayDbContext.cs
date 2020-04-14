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
                .UseSqlServer(@"Server=.\MSSQLSERVER2017;Initial Catalog=Main_MadPay724db;User Id=keyvan_madpay724;Password=3?55Tilp;");
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
