using MadPay724.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MadPay724.Data.DatabaseContext
{
    public class MadpayDbContext : IdentityDbContext<User,Role,string,
    IdentityUserClaim<string>,UserRole,IdentityUserLogin<string>,
    IdentityRoleClaim<string>,IdentityUserToken<string>>
    {
        public MadpayDbContext()
        {
        }
        public MadpayDbContext(DbContextOptions<MadpayDbContext> opt) : base(opt)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer(@"Data Source=KEY1-LAB\MSSQLSERVER2016;Initial Catalog=MadPay724db;Integrated Security=True;MultipleActiveResultSets=True;");
        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<BankCard> BankCards { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Wallet> Wallets { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
            //builder.Entity<Wallet>(
            //    code =>
            //    {
            //              code.HasIndex(e => e.Code).IsUnique() ;
            //              code.Property(e => e.Code).ValueGeneratedOnAdd();
            //    });
        }


    }
}
