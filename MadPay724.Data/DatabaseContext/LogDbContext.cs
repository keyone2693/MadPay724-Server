using MadPay724.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.DatabaseContext
{
    public class LogDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer(@"Data Source=KEY1-LAB\MSSQLSERVER2016;Initial Catalog=Logdb;Integrated Security=True;MultipleActiveResultSets=True;");
        }

        public DbSet<Log> Logs { get; set; }
    }
}
