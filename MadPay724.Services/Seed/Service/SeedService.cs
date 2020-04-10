using System;
using MadPay724.Data.Models.MainDB;
using MadPay724.Services.Seed.Interface;
using Newtonsoft.Json;
using System.Collections.Generic;
using MadPay724.Common.Helpers.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using MadPay724.Services.Site.Panel.Auth.Interface;
using MadPay724.Data.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.IO;

namespace MadPay724.Services.Seed.Service
{
    public class SeedService : ISeedService
    {
        private readonly Main_MadPayDbContext _dbMain;
        private readonly Financial_MadPayDbContext _dbFinancial;
        private readonly Log_MadPayDbContext _dbLog;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUtilities _utilities;
        private readonly IAuthService _authService;

        public SeedService(UserManager<User> userManager, RoleManager<Role> roleManager, IUtilities utilities,
            IAuthService authService, Main_MadPayDbContext dbMain,
            Financial_MadPayDbContext dbFinancial,
            Log_MadPayDbContext dbLog)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _utilities = utilities;
            _authService = authService;
            _dbMain = dbMain;
            _dbFinancial = dbFinancial;
            _dbLog = dbLog;

        }


        public void SeedUsers()
        {

            System.Console.WriteLine("Adding Migrations ...");


            _dbMain.Database.Migrate();
            _dbFinancial.Database.Migrate();
            _dbLog.Database.Migrate();


            if (!_dbMain.Settings.Any(p=>p.Id == 1))
            {
                _dbMain.Settings.Add(new Setting
                {
                    CloudinaryCloudName = "keyone2693",
                    CloudinaryAPIKey = "392574657416383",
                    CloudinaryAPISecret = "J7nBtA2rjiyvYmhYUWwe8-sATCs",
                    UploadLocal = true
                });

                _dbMain.SaveChanges();
            }

            if (!_userManager.Users.Any())
            {
                System.Console.WriteLine("Adding Data ...");

                //var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //var filePath = buildDir + @"\UserSeedData.json";
                //var userData = System.IO.File.ReadAllText(filePath);

                var userData = "[  {    \"Username\": \"popecopeland@barkarama.com\",    \"Name\": \"Stanton Hatfield\",    \"PhoneNumber\": \"+98 (860) 501-3004\",    \"Address\": \"733 Lombardy Street, Emison, Federated States Of Micronesia, 6618\",    \"Gender\": true,    \"City\": \"Cowiche\",    \"IsActive\": true,    \"Status\": false,    \"DateOfBirth\": \"2001-03-03\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/men/40.jpg\",        \"Alt\": \"Fugiat ut incididunt quis exercitation nisi est aute commodo dolore laborum fugiat veniam aliqua.\",        \"IsMain\": true,        \"Description\": \"Ullamco sunt qui pariatur incididunt.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Anastasia}\",        \"OwnerName\": \"Matthews Landry\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": 'اصلی ماد پی',        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"matthewslandry@barkarama.com\",    \"Name\": \"Cruz Lyons\",    \"PhoneNumber\": \"+98 (962) 580-2615\",    \"Address\": \"338 Tampa Court, Cumberland, Utah, 4341\",    \"Gender\": true,    \"City\": \"Glendale\",    \"IsActive\": false,    \"Status\": false,    \"DateOfBirth\": \"1995-09-17\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/men/92.jpg\",        \"Alt\": \"Veniam reprehenderit occaecat laborum non tempor ipsum irure velit elit deserunt fugiat.\",        \"IsMain\": true,        \"Description\": \"Lorem duis ea labore aliquip ullamco.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Newman}\",        \"OwnerName\": \"Hays Mathis\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"haysmathis@barkarama.com\",    \"Name\": \"Holloway Vasquez\",    \"PhoneNumber\": \"+98 (897) 595-3446\",    \"Address\": \"962 Sutton Street, Grimsley, Maryland, 8508\",    \"Gender\": true,    \"City\": \"Evergreen\",    \"IsActive\": true,    \"Status\": false,    \"DateOfBirth\": \"1951-11-23\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/men/70.jpg\",        \"Alt\": \"Nulla ea eiusmod officia consequat aliquip deserunt magna adipisicing.\",        \"IsMain\": true,        \"Description\": \"Adipisicing veniam magna veniam minim eiusmod elit culpa elit dolore ex pariatur ullamco labore.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Madelyn}\",        \"OwnerName\": \"Edwards Ayers\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"edwardsayers@barkarama.com\",    \"Name\": \"Fitzgerald Taylor\",    \"PhoneNumber\": \"+98 (948) 528-3044\",    \"Address\": \"693 Pierrepont Street, Bayview, Northern Mariana Islands, 4905\",    \"Gender\": true,    \"City\": \"Highland\",    \"IsActive\": true,    \"Status\": true,    \"DateOfBirth\": \"1966-09-17\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/men/59.jpg\",        \"Alt\": \"Ex sunt sint nostrud aliqua.\",        \"IsMain\": true,        \"Description\": \"Fugiat magna veniam cillum nostrud adipisicing nostrud irure.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Boyer}\",        \"OwnerName\": \"Wiley Thomas\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"wileythomas@barkarama.com\",    \"Name\": \"Wooten Holland\",    \"PhoneNumber\": \"+98 (899) 428-2702\",    \"Address\": \"376 Monitor Street, Westmoreland, Nevada, 5732\",    \"Gender\": true,    \"City\": \"Fairfield\",    \"IsActive\": true,    \"Status\": false,    \"DateOfBirth\": \"1999-01-12\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/men/75.jpg\",        \"Alt\": \"Minim adipisicing do ut nulla fugiat reprehenderit tempor nisi.\",        \"IsMain\": true,        \"Description\": \"Velit esse irure est voluptate consequat et aute.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Jane}\",        \"OwnerName\": \"Love Ochoa\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"loveochoa@barkarama.com\",    \"Name\": \"Gabriela Sanchez\",    \"PhoneNumber\": \"+98 (935) 556-3405\",    \"Address\": \"452 Fairview Place, Leland, Georgia, 4226\",    \"Gender\": false,    \"City\": \"Eastmont\",    \"IsActive\": false,    \"Status\": false,    \"DateOfBirth\": \"1978-04-26\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/women/5.jpg\",        \"Alt\": \"Reprehenderit ipsum exercitation non est duis anim consequat deserunt eiusmod ex dolor.\",        \"IsMain\": true,        \"Description\": \"Deserunt enim in ullamco ea pariatur pariatur consequat ea aliquip adipisicing proident ullamco.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Cox}\",        \"OwnerName\": \"Ofelia Valenzuela\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"ofeliavalenzuela@barkarama.com\",    \"Name\": \"Laurie Holland\",    \"PhoneNumber\": \"+98 (907) 577-3936\",    \"Address\": \"688 Nassau Avenue, Jackpot, Puerto Rico, 4376\",    \"Gender\": false,    \"City\": \"Fidelis\",    \"IsActive\": false,    \"Status\": true,    \"DateOfBirth\": \"2011-05-21\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/women/72.jpg\",        \"Alt\": \"Reprehenderit ad qui pariatur et fugiat quis deserunt ex ea reprehenderit tempor irure minim.\",        \"IsMain\": true,        \"Description\": \"Id quis in in culpa culpa reprehenderit mollit nulla.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Phillips}\",        \"OwnerName\": \"Lina Cummings\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"linacummings@barkarama.com\",    \"Name\": \"Heather Garrison\",    \"PhoneNumber\": \"+98 (820) 593-2877\",    \"Address\": \"677 Ovington Avenue, Hannasville, Wisconsin, 8346\",    \"Gender\": false,    \"City\": \"Harleigh\",    \"IsActive\": true,    \"Status\": false,    \"DateOfBirth\": \"1999-04-06\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/women/53.jpg\",        \"Alt\": \"Occaecat ut amet nostrud consectetur in.\",        \"IsMain\": true,        \"Description\": \"Adipisicing amet magna dolore reprehenderit ad laboris qui elit adipisicing minim.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Mcintyre}\",        \"OwnerName\": \"Olive Porter\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"oliveporter@barkarama.com\",    \"Name\": \"Delores Owen\",    \"PhoneNumber\": \"+98 (852) 491-3253\",    \"Address\": \"101 Stryker Court, Coral, Michigan, 2694\",    \"Gender\": false,    \"City\": \"Blandburg\",    \"IsActive\": true,    \"Status\": false,    \"DateOfBirth\": \"2007-09-11\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/women/79.jpg\",        \"Alt\": \"Est sunt cillum in irure consequat amet aute.\",        \"IsMain\": true,        \"Description\": \"Quis reprehenderit ex tempor ut excepteur dolore cillum.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Latasha}\",        \"OwnerName\": \"Kathy Brown\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  },  {    \"Username\": \"dfsfsdfsdfhybrown@barkarama.com\",    \"Name\": \"Claudia Allison\",    \"PhoneNumber\": \"+98 (963) 421-3361\",    \"Address\": \"369 Hope Street, Lydia, Tennessee, 7070\",    \"Gender\": false,    \"City\": \"Hiwasse\",    \"IsActive\": false,    \"Status\": false,    \"DateOfBirth\": \"1955-11-30\",    \"Photos\": [      {        \"Url\": \"https://randomuser.me/api/portraits/women/99.jpg\",        \"Alt\": \"Magna tempor aliqua sit sunt amet.\",        \"IsMain\": true,        \"Description\": \"Dolore Lorem elit adipisicing et deserunt quis laboris fugiat minim do.\"      }    ],    \"BankCards\": [      {        \"Approve\": false,        \"BankName\": \"Wilma}\",        \"OwnerName\": \"Meagan Burris\",        \"Shaba\": \"IR 0236598000215632014598002\",        \"CardNumber\": 5986320015498532,        \"ExpireDateMonth\": 98,        \"ExpireDateYear\": 4      }    ],    \"Notifications\": [      {        \"EnterEmail\": true,        \"EnterSms\": false,        \"EnterTelegram\": true,        \"ExitEmail\": true,        \"ExitSms\": false,        \"ExitTelegram\": true,        \"TicketEmail\": true,        \"TicketSms\": false,        \"TicketTelegram\": true,        \"LoginEmail\": true,        \"LoginSms\": false,        \"LoginTelegram\": true      }    ],    \"Wallets\": [      {        \"IsMain\": true,        \"IsSms\": false,        \"Name\": \"اصلی ماد پی\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      },      {        \"IsMain\": false,        \"IsSms\": true,        \"Name\": \"پیامک\",        \"Inventory\": 0,        \"InterMoney\": 0,        \"ExitMoney\": 0,        \"OnExitMoney\": 0      }    ]  }]";
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