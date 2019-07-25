using MadPay724.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MadPay724.Data.DatabaseContext
{
    public class MadpayDbContext : IdentityDbContext<User,Role, string,
       IdentityUserClaim<string>, UserRole ,IdentityUserLogin<string>, 
       IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer(@"Data Source=KEY1-LAB\MSSQLSERVER2016;Initial Catalog=MadPay724db;Integrated Security=True;MultipleActiveResultSets=True;");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<BankCard> BankCards { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}
