using System;
using MadPay724.Data.Models;
using MadPay724.Services.Seed.Interface;
using Newtonsoft.Json;
using System.Collections.Generic;
using MadPay724.Common.Helpers.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using MadPay724.Services.Site.Admin.Auth.Interface;

namespace MadPay724.Services.Seed.Service
{
    public class SeedService : ISeedService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUtilities _utilities;
        private readonly IAuthService _authService;

        public SeedService(UserManager<User> userManager, RoleManager<Role> roleManager, IUtilities utilities,
            IAuthService authService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _utilities = utilities;
            _authService = authService;
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
                    new Role{Name="AdminBlog"},
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
                    var photoToCreate = new Photo
                    {
                        UserId = adminUser.Id,
                        Url = "https://res.cloudinary.com/keyone2693/image/upload/v1561717720/768px-Circle-icons-profile.svg.png",
                        Description = "Profile Pic",
                        Alt = "Profile Pic",
                        IsMain = true,
                        PublicId = "0"
                    };

                    var notifyToCreate = new Notification
                    {
                        UserId = adminUser.Id,
                        EnterEmail = true,
                        EnterSms = false,
                        EnterTelegram = true,
                        ExitEmail = true,
                        ExitSms = false,
                        ExitTelegram = true,
                        LoginEmail = true,
                        LoginSms = false,
                        LoginTelegram = true,
                        TicketEmail = true,
                        TicketSms = false,
                        TicketTelegram = true
                    };

                    _authService.AddUserPreNeededAsync(photoToCreate, notifyToCreate,null,null).Wait();

                    var admin = _userManager.FindByNameAsync("admin@madpay724.com").Result;
                    _userManager.AddToRolesAsync(admin, new[] { "Admin", "Blog", "Accountant" }).Wait();
                }
                //Create AdminBlogUser
                var adminblogUser = new User
                {
                    UserName = "adminblog@madpay724.com",
                    Name = "AdminBlog",
                    Address = "No",
                    DateOfBirth = DateTime.Now,
                    LastActive = DateTime.Now,
                };
                IdentityResult resultAdminBlog = _userManager.CreateAsync(adminblogUser, "password").Result;

                if (resultAdminBlog.Succeeded)
                {
                    var photoToCreate = new Photo
                    {
                        UserId = adminblogUser.Id,
                        Url = "https://res.cloudinary.com/keyone2693/image/upload/v1561717720/768px-Circle-icons-profile.svg.png",
                        Description = "Profile Pic",
                        Alt = "Profile Pic",
                        IsMain = true,
                        PublicId = "0"
                    };

                    var notifyToCreate = new Notification
                    {
                        UserId = adminblogUser.Id,
                        EnterEmail = true,
                        EnterSms = false,
                        EnterTelegram = true,
                        ExitEmail = true,
                        ExitSms = false,
                        ExitTelegram = true,
                        LoginEmail = true,
                        LoginSms = false,
                        LoginTelegram = true,
                        TicketEmail = true,
                        TicketSms = false,
                        TicketTelegram = true
                    };

                    _authService.AddUserPreNeededAsync(photoToCreate, notifyToCreate, null, null).Wait();

                    var blog = _userManager.FindByNameAsync("adminblog@madpay724.com").Result;
                    _userManager.AddToRoleAsync(blog, "AdminBlog").Wait();
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
                    var photoToCreate = new Photo
                    {
                        UserId = blogUser.Id,
                        Url = "https://res.cloudinary.com/keyone2693/image/upload/v1561717720/768px-Circle-icons-profile.svg.png",
                        Description = "Profile Pic",
                        Alt = "Profile Pic",
                        IsMain = true,
                        PublicId = "0"
                    };

                    var notifyToCreate = new Notification
                    {
                        UserId = blogUser.Id,
                        EnterEmail = true,
                        EnterSms = false,
                        EnterTelegram = true,
                        ExitEmail = true,
                        ExitSms = false,
                        ExitTelegram = true,
                        LoginEmail = true,
                        LoginSms = false,
                        LoginTelegram = true,
                        TicketEmail = true,
                        TicketSms = false,
                        TicketTelegram = true
                    };

                    _authService.AddUserPreNeededAsync(photoToCreate, notifyToCreate, null, null).Wait();

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
                    var photoToCreate = new Photo
                    {
                        UserId = accountantUser.Id,
                        Url = "https://res.cloudinary.com/keyone2693/image/upload/v1561717720/768px-Circle-icons-profile.svg.png",
                        Description = "Profile Pic",
                        Alt = "Profile Pic",
                        IsMain = true,
                        PublicId = "0"
                    };

                    var notifyToCreate = new Notification
                    {
                        UserId = accountantUser.Id,
                        EnterEmail = true,
                        EnterSms = false,
                        EnterTelegram = true,
                        ExitEmail = true,
                        ExitSms = false,
                        ExitTelegram = true,
                        LoginEmail = true,
                        LoginSms = false,
                        LoginTelegram = true,
                        TicketEmail = true,
                        TicketSms = false,
                        TicketTelegram = true
                    };

                    _authService.AddUserPreNeededAsync(photoToCreate, notifyToCreate, null, null).Wait();
                    var accountant = _userManager.FindByNameAsync("accountant@madpay724.com").Result;
                    _userManager.AddToRoleAsync(accountant, "Accountant").Wait();
                }
            }
        }
    }

}