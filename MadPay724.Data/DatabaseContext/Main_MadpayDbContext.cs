using MadPay724.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MadPay724.Data.Models.MainDB;
using MadPay724.Data.Models.MainDB.UserModel;
using MadPay724.Data.Models.MainDB.Blog;

namespace MadPay724.Data.DatabaseContext
{
    public class Main_MadPayDbContext : IdentityDbContext<User,Role,string,
    IdentityUserClaim<string>,UserRole,IdentityUserLogin<string>,
    IdentityRoleClaim<string>,IdentityUserToken<string>>
    {
        public Main_MadPayDbContext()
        {
        }
        public Main_MadPayDbContext(DbContextOptions<Main_MadPayDbContext> opt) : base(opt)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer(@"Data Source=KEY1-LAB\MSSQLSERVER2016;Initial Catalog=Main_MadPay724db;Integrated Security=True;MultipleActiveResultSets=True;");
        }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<BankCard> BankCards { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketContent> TicketContents { get; set; }
        public DbSet<Gate> Gates { get; set; }
        public DbSet<EasyPay> EasyPays { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogGroup> BlogGroups { get; set; }


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

            builder.Entity<Blog>()
                .Property(x => x.Title)
                .IsConcurrencyToken();


            builder.Entity<Blog>()
                .Property(x => x.Timestamp)
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
                

        }


    }
}
