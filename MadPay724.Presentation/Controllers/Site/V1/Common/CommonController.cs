using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Common;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Common
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Common")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<CommonController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;
        private readonly IUtilities _utilities;

        public CommonController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<CommonController> logger, IUploadService uploadService,
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
        [Authorize(Policy = "AccessNotify")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Common.GetNotifications)]
        public async Task<IActionResult> GetNotifications(string id)
        {
            var res = new NotificationsCountDto();

            if (User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                res.UnVerifiedDocuments = await _db.DocumentRepository.GetCountAsync(p => p.Approve == 0);
                res.UnClosedTicketCount = await _db.TicketRepository.GetCountAsync(p => !p.Closed);
                //
                res.UnCheckedEntry = await _dbFinancial.EntryRepository.GetCountAsync(p => !p.IsApprove);
                res.UnSpecifiedEntry = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsApprove && !p.IsReject && !p.IsPardakht);
                res.UnVerifiedBankCardInPast7Days = await _db.BankCardRepository.GetCountAsync(p => !p.Approve && p.DateModified < DateTime.Now.AddDays(7));
                res.UnVerifiedGateInPast7Days = await _db.GateRepository.GetCountAsync(p => !p.IsActive && p.DateModified < DateTime.Now.AddDays(7));
                //
                res.UnVerifiedBlogCount = await _db.BlogRepository.GetCountAsync(p => !p.Status);
                
            }
            else if (User.HasClaim(ClaimTypes.Role, "Accountant"))
            {
                res.UnCheckedEntry = await _dbFinancial.EntryRepository.GetCountAsync(p => !p.IsApprove);
                res.UnSpecifiedEntry = await _dbFinancial.EntryRepository.GetCountAsync(p => p.IsApprove && !p.IsReject && !p.IsPardakht);
                res.UnVerifiedBankCardInPast7Days = await _db.BankCardRepository.GetCountAsync(p => !p.Approve && p.DateModified < DateTime.Now.AddDays(7));
                res.UnVerifiedGateInPast7Days = await _db.GateRepository.GetCountAsync(p => !p.IsActive && p.DateModified < DateTime.Now.AddDays(7));
            }
            else if (User.HasClaim(ClaimTypes.Role, "AdminBlog"))
            {
                res.UnVerifiedBlogCount = await _db.BlogRepository.GetCountAsync(p => !p.Status);
            }
            else if (User.HasClaim(ClaimTypes.Role, "User"))
            {
                res.UnClosedTicketCount = await _db.TicketRepository.GetCountAsync(p => p.UserId == id && !p.Closed);
            }
            return Ok(res);
        }
    }
}