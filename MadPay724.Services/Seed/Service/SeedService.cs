using System;
using MadPay724.Common.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Seed.Interface;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Common.Helpers.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace MadPay724.Services.Seed.Service
{
    public class SeedService : ISeedService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUtilities _utilities;

        public SeedService(UserManager<User> userManager, RoleManager<Role> roleManager, IUtilities utilities)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _utilities = utilities;
        }


        public void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("wwwroot/Files/Json/Seed/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<IList<User>>(userData);

                var roles = new List<Role>
                {
                    new Role{Name="Admin"},
                    new Role{Name="User"},
                    new Role{Name="Blog"},
                    new Role{Name="Accountant"}
                };

                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }

                foreach (var user in users)
                {
                    user.UserName = user.UserName.ToLower();
                    _userManager.CreateAsync(user, "password").Wait();
                    _userManager.AddToRoleAsync(user, "User").Wait();
                }


                //Create AdminUser
                var adminUser = new User
                {
                    
                    UserName = "admin@madpay724.com",
                    Name = "Admin",
                    Address ="No",
                    DateOfBirth = DateTime.Now,
                    LastActive = DateTime.Now,
                };
                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("admin@madpay724.com").Result;
                    _userManager.AddToRolesAsync(admin, new[] { "Admin", "Blog", "Accountant" }).Wait();
                }
                //Create BlogUser
                var blogUser = new User
                {
                    UserName = "blog@madpay724.com",
                    Name = "Blog",
                    Address = "No",
                    DateOfBirth = DateTime.Now,
                    LastActive = DateTime.Now,
                };
                IdentityResult resultBlog = _userManager.CreateAsync(blogUser, "password").Result;

                if (resultBlog.Succeeded)
                {
                    var blog = _userManager.FindByNameAsync("blog@madpay724.com").Result;
                    _userManager.AddToRoleAsync(blog, "Blog").Wait();
                }
                //Create AccountantUser
                var accountantUser = new User
                {
                    UserName = "accountant@madpay724.com",
                    Name = "Accountant",
                    Address = "No",
                    DateOfBirth = DateTime.Now,
                    LastActive = DateTime.Now,
                };
                IdentityResult resultAccountant = _userManager.CreateAsync(accountantUser, "password").Result;

                if (resultAccountant.Succeeded)
                {
                    var accountant = _userManager.FindByNameAsync("accountant@madpay724.com").Result;
                    _userManager.AddToRoleAsync(accountant, "Accountant").Wait();
                }
            }
        }
    }

}