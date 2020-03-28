using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Data.Dtos.Site.Panel.Common;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Models.MainDB.Blog;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Common.Routes.V1.Site;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.V1.Panel.Common
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Common")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;
        private readonly IUtilities _utilities;

        public DashboardController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<DashboardController> logger, IUploadService uploadService,
            IWebHostEnvironment env, IUtilities utilities, IUnitOfWork<Financial_MadPayDbContext> dbFinancial)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
            _utilities = utilities;
        }
        //------------------------------
        [Authorize(Policy = "RequireUserRole")]
        [HttpGet(SiteV1Routes.Dashboard.GetUserDashboard)]
        public async Task<IActionResult> GetUserDashboard(string userId)
        {
            var res = new UserDashboardDto();

            res.UnClosedTicketCount = await _db.TicketRepository.GetCountAsync(p => p.UserId == userId && !p.Closed);
            res.ClosedTicketCount = await _db.TicketRepository.GetCountAsync(p => p.UserId == userId && p.Closed);
            res.Last5Tickets = await _db.TicketRepository.GetManyAsync(p => p.UserId == userId, null, "TicketContents", 5);
            res.TotalInventory = await _db.WalletRepository.GetSumAsync(p => p.UserId == userId, p => p.Inventory);
            res.Inventory5Days = new DaysForReturnDto
            {
                Day1 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date && p.IsPardakht, p => p.Price),
                Day2 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.IsPardakht, p => p.Price),
                Day3 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.IsPardakht, p => p.Price),
                Day4 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.IsPardakht, p => p.Price),
                Day5 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.IsPardakht, p => p.Price),
            };
            res.TotalInterMoney = await _db.WalletRepository.GetSumAsync(p => p.UserId == userId, p => p.InterMoney);
            res.InterMoney5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date && p.Status, p => p.Price),
                Day2 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.Status, p => p.Price),
                Day3 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.Status, p => p.Price),
                Day4 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.Status, p => p.Price),
                Day5 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.Status, p => p.Price),
            };
            res.TotalExitMoney = await _db.WalletRepository.GetSumAsync(p => p.UserId == userId, p => p.ExitMoney);
            res.ExitMoney5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date && p.IsPardakht, p => p.Price),
                Day2 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.IsPardakht, p => p.Price),
                Day3 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.IsPardakht, p => p.Price),
                Day4 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.IsPardakht, p => p.Price),
                Day5 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.IsPardakht, p => p.Price),
            };
            res.Factor12Months = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month && p.Status, p => p.EndPrice),

                Day2 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month && p.Status, p => p.EndPrice),

                Day3 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month && p.Status, p => p.EndPrice),

                Day4 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month && p.Status, p => p.EndPrice),

                Day5 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month && p.Status, p => p.EndPrice),

                Day6 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month && p.Status, p => p.EndPrice),

                Day7 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month && p.Status, p => p.EndPrice),

                Day8 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month && p.Status, p => p.EndPrice),

                Day9 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month && p.Status, p => p.EndPrice),

                Day10 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month && p.Status, p => p.EndPrice),

                Day11 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month && p.Status, p => p.EndPrice),

                Day12 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month && p.Status, p => p.EndPrice),
            };
            res.Last7Factors = await _dbFinancial.FactorRepository.GetManyAsync(p => p.UserId == userId, p => p.OrderBy(s => s.DateModified), "", 7);
            res.TotalSuccessEntry = await _dbFinancial.EntryRepository.GetSumAsync(p => p.UserId == userId && p.IsPardakht, p => p.Price);
            res.Last10Entries = await _dbFinancial.EntryRepository.GetManyAsync(p => p.UserId == userId, p => p.OrderBy(s => s.DateModified), "", 10);

            res.TotalFactorDaramad = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.Status && p.Kind == 1, p => p.EndPrice);

            res.TotalEasyPayDaramad = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.Status && p.Kind == 2, p => p.EndPrice);

            res.TotalSupportDaramad = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.Status && p.Kind == 3, p => p.EndPrice);

            res.TotalIncInventoryDaramad = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.Status && p.Kind == 4, p => p.EndPrice);

            res.TotalSuccessFactor = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.Status, p => p.EndPrice);

            return Ok(res);
        }

        [Authorize(Policy = "AccessBloger")]
        [HttpGet(SiteV1Routes.Dashboard.GetBlogDashboard)]
        public async Task<IActionResult> GetBlogDashboard(string userId)
        {
            var res = new BlogDashboardDto();

            if (User.HasClaim(ClaimTypes.Role, "AdminBlog"))
            {
                res.TotalBlogCount = await _db.BlogRepository.GetCountAsync(null);
                res.ApprovedBlogCount = await _db.BlogRepository.GetCountAsync(p => p.Status);
                res.UnApprovedBlogCount = await _db.BlogRepository.GetCountAsync(p => !p.Status);

                res.TotalBlog5Days = new DaysForReturnDto
                {
                    Day1 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.Date),
                    Day2 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date),
                    Day3 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date),
                    Day4 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date),
                    Day5 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date),
                };
                res.ApprovedBlog5Days = new DaysForReturnDto
                {
                    Day1 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.Date && p.Status),
                    Day2 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.Status),
                    Day3 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.Status),
                    Day4 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.Status),
                    Day5 = await _db.BlogRepository
                .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.Status),
                };
                res.UnApprovedBlog5Days = new DaysForReturnDto
                {
                    Day1 = await _db.BlogRepository
               .GetCountAsync(p => p.DateModified.Date == DateTime.Now.Date && !p.Status),
                    Day2 = await _db.BlogRepository
               .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date && !p.Status),
                    Day3 = await _db.BlogRepository
               .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && !p.Status),
                    Day4 = await _db.BlogRepository
               .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && !p.Status),
                    Day5 = await _db.BlogRepository
               .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date && !p.Status),
                };

                var blogs = await _db.BlogRepository.GetManyAsync(null, s => s.OrderBy(p => p.DateModified), "User,BlogGroup", 7);
                res.Last7Blogs = new List<BlogForReturnDto>();
                foreach (var blog in blogs)
                {
                    res.Last7Blogs.Add(_mapper.Map<BlogForReturnDto>(blog));
                }



                var users = await _db.UserRepository
                    .GetManyAsync(p => p.UserRoles.Any(s => s.Role.Name == "Blog"), p => p.OrderByDescending(s => s.Blogs.Count()), "Blogs", 12);

                res.Last12UserBlogInfo = new List<UserBlogInfoDto>();

                foreach (var user in users)
                {
                    res.Last12UserBlogInfo.Add(new UserBlogInfoDto
                    {
                        Name = user.Name,
                        TotalBlog = user.Blogs.Count,
                        ApprovedBlog = user.Blogs.Count(p => p.Status),
                        UnApprovedBlog = user.Blogs.Count(p => !p.Status)
                    });
                }
            }
            else // Blog
            {
                res.TotalBlogCount = await _db.BlogRepository.GetCountAsync(p => p.UserId == userId);
                res.ApprovedBlogCount = await _db.BlogRepository.GetCountAsync(p => p.UserId == userId && p.Status);
                res.UnApprovedBlogCount = await _db.BlogRepository.GetCountAsync(p => p.UserId == userId && !p.Status);

                res.TotalBlog5Days = new DaysForReturnDto
                {
                    Day1 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date),
                    Day2 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date),
                    Day3 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date),
                    Day4 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date),
                    Day5 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date),
                };
                res.ApprovedBlog5Days = new DaysForReturnDto
                {
                    Day1 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date && p.Status),
                    Day2 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.Status),
                    Day3 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.Status),
                    Day4 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.Status),
                    Day5 = await _db.BlogRepository
                .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.Status),
                };
                res.UnApprovedBlog5Days = new DaysForReturnDto
                {
                    Day1 = await _db.BlogRepository
               .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date && !p.Status),
                    Day2 = await _db.BlogRepository
               .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date && !p.Status),
                    Day3 = await _db.BlogRepository
               .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date && !p.Status),
                    Day4 = await _db.BlogRepository
               .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date && !p.Status),
                    Day5 = await _db.BlogRepository
               .GetCountAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date && !p.Status),
                };


                var blogs = await _db.BlogRepository.GetManyAsync(p => p.UserId == userId, s => s.OrderBy(p => p.DateModified), "User,BlogGroup", 7);
                res.Last7Blogs = new List<BlogForReturnDto>();
                foreach (var blog in blogs)
                {
                    res.Last7Blogs.Add(_mapper.Map<BlogForReturnDto>(blog));
                }

                res.Last12UserBlogInfo = new List<UserBlogInfoDto>();

            }
            return Ok(res);
        }

        [Authorize(Policy = "RequireAccountantRole")]
        [HttpGet(SiteV1Routes.Dashboard.GetAccountantDashboard)]
        public async Task<IActionResult> GetAccountantDashboard()
        {
            var res = new AccountantDashboardDto();

            res.TotalSuccessEntry = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsPardakht);
            res.TotalSuccessEntryPrice = await _dbFinancial.EntryRepository.GetSumAsync(p => p.IsPardakht, p => p.Price);

            res.TotalEntryApprove = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsApprove);
            res.TotalEntryUnApprove = await _dbFinancial.EntryRepository.GetCountAsync(p => !p.IsApprove);
            res.TotalEntryPardakht = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsPardakht);
            res.TotalEntryUnPardakht = await _dbFinancial.EntryRepository.GetCountAsync(p => !p.IsPardakht);
            res.TotalEntryReject = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsReject);
            res.TotalEntryUnReject = await _dbFinancial.EntryRepository.GetCountAsync(p => !p.IsReject);

            res.TotalFactor = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status && p.Kind == 1);
            res.TotalFactorPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status && p.Kind == 1, p => p.EndPrice);

            res.TotalEasyPay = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status && p.Kind == 2);
            res.TotalEasyPayPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status && p.Kind == 2, p => p.EndPrice);

            res.TotalSupport = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status && p.Kind == 3);
            res.TotalSupportPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status && p.Kind == 3, p => p.EndPrice);

            res.TotalIncInventory = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status && p.Kind == 4);
            res.TotalIncInventoryPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status && p.Kind == 4, p => p.EndPrice);

            res.TotalSuccessFactor = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status);
            res.TotalSuccessFactorPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status, p => p.EndPrice);


            res.Entry5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.Date && p.IsPardakht, p => p.Price),
                Day2 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.IsPardakht, p => p.Price),
                Day3 =  await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.IsPardakht, p => p.Price),
                Day4 =  await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.IsPardakht, p => p.Price),
                Day5 =  await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.IsPardakht, p => p.Price),
            };
            res.Factor5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.Date && p.Status, p => p.EndPrice),
                Day2 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.Status, p => p.EndPrice),
                Day3 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.Status, p => p.EndPrice),
                Day4 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.Status, p => p.EndPrice),
                Day5 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.Status, p => p.EndPrice),
            };
            res.Factor12Months = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month && p.Status, p => p.EndPrice),

                Day2 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month && p.Status, p => p.EndPrice),

                Day3 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month && p.Status, p => p.EndPrice),

                Day4 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month && p.Status, p => p.EndPrice),

                Day5 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month && p.Status, p => p.EndPrice),

                Day6 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month && p.Status, p => p.EndPrice),

                Day7 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month && p.Status, p => p.EndPrice),

                Day8 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month && p.Status, p => p.EndPrice),

                Day9 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month && p.Status, p => p.EndPrice),

                Day10 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month && p.Status, p => p.EndPrice),

                Day11 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month && p.Status, p => p.EndPrice),

                Day12 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month && p.Status, p => p.EndPrice),
            };

            res.Entry12Months = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month && p.IsPardakht, p => p.Price),

                Day2 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month && p.IsPardakht, p => p.Price),

                Day3 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month && p.IsPardakht, p => p.Price),

                Day4 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month && p.IsPardakht, p => p.Price),

                Day5 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month && p.IsPardakht, p => p.Price),

                Day6 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month && p.IsPardakht, p => p.Price),

                Day7 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month && p.IsPardakht, p => p.Price),

                Day8 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month && p.IsPardakht, p => p.Price),

                Day9 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month && p.IsPardakht, p => p.Price),

                Day10 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month && p.IsPardakht, p => p.Price),

                Day11 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month && p.IsPardakht, p => p.Price),

                Day12 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month && p.IsPardakht, p => p.Price),
            };

            res.BankCard12Months = new DaysForReturnDto
            {
                Day1 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month),

                Day2 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month),

                Day3 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month),

                Day4 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month),

                Day5 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month),

                Day6 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month),

                Day7 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month),

                Day8 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month),

                Day9 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month),

                Day10 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month),

                Day11 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month),

                Day12 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month),
            };

            res.Gate12Months = new DaysForReturnDto
            {
                Day1 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month),

                Day2 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month),

                Day3 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month),

                Day4 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month),

                Day5 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month),

                Day6 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month),

                Day7 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month),

                Day8 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month),

                Day9 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month),

                Day10 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month),

                Day11 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month),

                Day12 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month),
            };

            res.Wallet12Months = new DaysForReturnDto
            {
                Day1 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month),

                Day2 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month),

                Day3 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month),

                Day4 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month),

                Day5 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month),

                Day6 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month),

                Day7 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month),

                Day8 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month),

                Day9 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month),

                Day10 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month),

                Day11 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month),

                Day12 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month),
            };

            res.Last7Factors = await _dbFinancial.FactorRepository.GetManyAsync(null, p => p.OrderBy(s => s.DateModified), "", 7);
            res.Last7Entries = await _dbFinancial.EntryRepository.GetManyAsync(null, p => p.OrderBy(s => s.DateModified), "", 7);

       
            return Ok(res);
    }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(SiteV1Routes.Dashboard.GetAdminDashboard)]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var res = new AdminDashboardDto();

            #region Acc

            res.TotalSuccessEntry = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsPardakht);
            res.TotalSuccessEntryPrice = await _dbFinancial.EntryRepository.GetSumAsync(p => p.IsPardakht, p => p.Price);

            res.TotalEntryApprove = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsApprove);
            res.TotalEntryUnApprove = await _dbFinancial.EntryRepository.GetCountAsync(p => !p.IsApprove);
            res.TotalEntryPardakht = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsPardakht);
            res.TotalEntryUnPardakht = await _dbFinancial.EntryRepository.GetCountAsync(p => !p.IsPardakht);
            res.TotalEntryReject = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsReject);
            res.TotalEntryUnReject = await _dbFinancial.EntryRepository.GetCountAsync(p => !p.IsReject);

            res.TotalFactor = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status && p.Kind == 1);
            res.TotalFactorPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status && p.Kind == 1, p => p.EndPrice);

            res.TotalEasyPay = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status && p.Kind == 2);
            res.TotalEasyPayPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status && p.Kind == 2, p => p.EndPrice);

            res.TotalSupport = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status && p.Kind == 3);
            res.TotalSupportPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status && p.Kind == 3, p => p.EndPrice);

            res.TotalIncInventory = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status && p.Kind == 4);
            res.TotalIncInventoryPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status && p.Kind == 4, p => p.EndPrice);

            res.TotalSuccessFactor = await _dbFinancial.FactorRepository.GetCountAsync(p => p.Status);
            res.TotalSuccessFactorPrice = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status, p => p.EndPrice);

            res.Entry5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.Date && p.IsPardakht, p => p.Price),
                Day2 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.IsPardakht, p => p.Price),
                Day3 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.IsPardakht, p => p.Price),
                Day4 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.IsPardakht, p => p.Price),
                Day5 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.IsPardakht, p => p.Price),
            };
            res.Factor5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.Date && p.Status, p => p.EndPrice),
                Day2 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.Status, p => p.EndPrice),
                Day3 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.Status, p => p.EndPrice),
                Day4 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.Status, p => p.EndPrice),
                Day5 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.Status, p => p.EndPrice),
            };
            res.Factor12Months = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month && p.Status, p => p.EndPrice),

                Day2 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month && p.Status, p => p.EndPrice),

                Day3 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month && p.Status, p => p.EndPrice),

                Day4 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month && p.Status, p => p.EndPrice),

                Day5 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month && p.Status, p => p.EndPrice),

                Day6 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month && p.Status, p => p.EndPrice),

                Day7 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month && p.Status, p => p.EndPrice),

                Day8 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month && p.Status, p => p.EndPrice),

                Day9 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month && p.Status, p => p.EndPrice),

                Day10 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month && p.Status, p => p.EndPrice),

                Day11 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month && p.Status, p => p.EndPrice),

                Day12 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month && p.Status, p => p.EndPrice),
            };
            res.Entry12Months = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month && p.IsPardakht, p => p.Price),

                Day2 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month && p.IsPardakht, p => p.Price),

                Day3 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month && p.IsPardakht, p => p.Price),

                Day4 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month && p.IsPardakht, p => p.Price),

                Day5 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month && p.IsPardakht, p => p.Price),

                Day6 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month && p.IsPardakht, p => p.Price),

                Day7 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month && p.IsPardakht, p => p.Price),

                Day8 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month && p.IsPardakht, p => p.Price),

                Day9 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month && p.IsPardakht, p => p.Price),

                Day10 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month && p.IsPardakht, p => p.Price),

                Day11 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month && p.IsPardakht, p => p.Price),

                Day12 = await _dbFinancial.EntryRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month && p.IsPardakht, p => p.Price),
            };
            res.BankCard12Months = new DaysForReturnDto
            {
                Day1 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month),

                Day2 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month),

                Day3 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month),

                Day4 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month),

                Day5 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month),

                Day6 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month),

                Day7 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month),

                Day8 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month),

                Day9 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month),

                Day10 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month),

                Day11 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month),

                Day12 = await _db.BankCardRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month),
            };
            res.Gate12Months = new DaysForReturnDto
            {
                Day1 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month),

                Day2 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month),

                Day3 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month),

                Day4 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month),

                Day5 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month),

                Day6 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month),

                Day7 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month),

                Day8 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month),

                Day9 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month),

                Day10 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month),

                Day11 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month),

                Day12 = await _db.GateRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month),
            };
            res.Wallet12Months = new DaysForReturnDto
            {
                Day1 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month),

                Day2 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month),

                Day3 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month),

                Day4 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month),

                Day5 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month),

                Day6 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month),

                Day7 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month),

                Day8 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month),

                Day9 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month),

                Day10 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month),

                Day11 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month),

                Day12 = await _db.WalletRepository.GetCountAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month),
            };
            res.Last7Factors = await _dbFinancial.FactorRepository.GetManyAsync(null, p => p.OrderBy(s => s.DateModified), "", 7);
            res.Last7Entries = await _dbFinancial.EntryRepository.GetManyAsync(null, p => p.OrderBy(s => s.DateModified), "", 7);

            #endregion

            #region blog
            res.TotalBlogCount = await _db.BlogRepository.GetCountAsync(null);
            res.ApprovedBlogCount = await _db.BlogRepository.GetCountAsync(p => p.Status);
            res.UnApprovedBlogCount = await _db.BlogRepository.GetCountAsync(p => !p.Status);

            res.TotalBlog5Days = new DaysForReturnDto
            {
                Day1 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.Date),
                Day2 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date),
                Day3 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date),
                Day4 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date),
                Day5 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date),
            };
            res.ApprovedBlog5Days = new DaysForReturnDto
            {
                Day1 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.Date && p.Status),
                Day2 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.Status),
                Day3 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.Status),
                Day4 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.Status),
                Day5 = await _db.BlogRepository
            .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.Status),
            };
            res.UnApprovedBlog5Days = new DaysForReturnDto
            {
                Day1 = await _db.BlogRepository
           .GetCountAsync(p => p.DateModified.Date == DateTime.Now.Date && !p.Status),
                Day2 = await _db.BlogRepository
           .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-1).Date && !p.Status),
                Day3 = await _db.BlogRepository
           .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && !p.Status),
                Day4 = await _db.BlogRepository
           .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && !p.Status),
                Day5 = await _db.BlogRepository
           .GetCountAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date && !p.Status),
            };

            var blogs = await _db.BlogRepository.GetManyAsync(null, s => s.OrderBy(p => p.DateModified), "User,BlogGroup", 7);
            res.Last7Blogs = new List<BlogForReturnDto>();
            foreach (var blog in blogs)
            {
                res.Last7Blogs.Add(_mapper.Map<BlogForReturnDto>(blog));
            }



            var users = await _db.UserRepository
                .GetManyAsync(p => p.UserRoles.Any(s => s.Role.Name == "Blog"), p => p.OrderByDescending(s => s.Blogs.Count()), "Blogs", 12);

            res.Last12UserBlogInfo = new List<UserBlogInfoDto>();

            foreach (var user in users)
            {
                res.Last12UserBlogInfo.Add(new UserBlogInfoDto
                {
                    Name = user.Name,
                    TotalBlog = user.Blogs.Count,
                    ApprovedBlog = user.Blogs.Count(p => p.Status),
                    UnApprovedBlog = user.Blogs.Count(p => !p.Status)
                });
            }

            #endregion

            #region user 
            res.UnClosedTicketCount = await _db.TicketRepository.GetCountAsync(p => !p.Closed);
            res.ClosedTicketCount = await _db.TicketRepository.GetCountAsync(p =>  p.Closed);
            res.Last5Tickets = await _db.TicketRepository.GetManyAsync(null, null, "TicketContents", 5);
            res.TotalInventory = await _db.WalletRepository.GetSumAsync(null, p => p.Inventory);
            res.Inventory5Days = new DaysForReturnDto
            {
                Day1 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.Date && p.IsPardakht, p => p.Price),
                Day2 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p =>   p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.IsPardakht, p => p.Price),
                Day3 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p =>   p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.IsPardakht, p => p.Price),
                Day4 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.IsPardakht, p => p.Price),
                Day5 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.IsPardakht, p => p.Price),
            };
            res.TotalInterMoney = await _db.WalletRepository.GetSumAsync(null, p => p.InterMoney);
            res.InterMoney5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.Date && p.Status, p => p.Price),
                Day2 = await _dbFinancial.FactorRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.Status, p => p.Price),
                Day3 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.Status, p => p.Price),
                Day4 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.Status, p => p.Price),
                Day5 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.Status, p => p.Price),
            };
            res.TotalExitMoney = await _db.WalletRepository.GetSumAsync(null, p => p.ExitMoney);
            res.ExitMoney5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.Date && p.IsPardakht, p => p.Price),
                Day2 = await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.IsPardakht, p => p.Price),
                Day3 = await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.IsPardakht, p => p.Price),
                Day4 = await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.IsPardakht, p => p.Price),
                Day5 = await _dbFinancial.EntryRepository
                .GetSumAsync(p =>  p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.IsPardakht, p => p.Price),
            };
            res.Factor12Months = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository.GetSumAsync(p=> p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month && p.Status, p => p.EndPrice),

                Day2 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                 p.DateModified.Month == DateTime.Now.AddMonths(-1).Month && p.Status, p => p.EndPrice),

                Day3 = await _dbFinancial.FactorRepository.GetSumAsync(p =>  p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month && p.Status, p => p.EndPrice),

                Day4 = await _dbFinancial.FactorRepository.GetSumAsync(p =>  p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month && p.Status, p => p.EndPrice),

                Day5 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-4).Year
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month && p.Status, p => p.EndPrice),

                Day6 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month && p.Status, p => p.EndPrice),

                Day7 = await _dbFinancial.FactorRepository.GetSumAsync(p =>  p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month && p.Status, p => p.EndPrice),

                Day8 = await _dbFinancial.FactorRepository.GetSumAsync(p =>  p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month && p.Status, p => p.EndPrice),

                Day9 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
             && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month && p.Status, p => p.EndPrice),

                Day10 = await _dbFinancial.FactorRepository.GetSumAsync(p =>  p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
           && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month && p.Status, p => p.EndPrice),

                Day11 = await _dbFinancial.FactorRepository.GetSumAsync(p =>  p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
         && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month && p.Status, p => p.EndPrice),

                Day12 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
       && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month && p.Status, p => p.EndPrice),
            };
            res.Last7Factors = await _dbFinancial.FactorRepository.GetManyAsync( null, p => p.OrderBy(s => s.DateModified), "", 7);
            res.TotalSuccessEntry = await _dbFinancial.EntryRepository.GetSumAsync(p => p.IsPardakht, p => p.Price);

            res.TotalSuccessFactor = await _dbFinancial.FactorRepository.GetSumAsync(p => p.Status, p => p.EndPrice);

            #endregion

            return Ok(res);
        }
    }
}