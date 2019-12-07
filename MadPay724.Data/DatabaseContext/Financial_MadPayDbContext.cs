using MadPay724.Data.Models.FinancialDB.Accountant;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.DatabaseContext
{
   public class Financial_MadPayDbContext : DbContext
    {
        public Financial_MadPayDbContext()
        {
        }
        public Financial_MadPayDbContext(DbContextOptions<Financial_MadPayDbContext> opt) : base(opt)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer(@"Data Source=KEY1-LAB\MSSQLSERVER2016;Initial Catalog=Financial_MadPay724db;Integrated Security=True;MultipleActiveResultSets=True;");
        }

        public DbSet<Factor> Factors { get; set; }
        public DbSet<Entry> Entries { get; set; }
    }
}
