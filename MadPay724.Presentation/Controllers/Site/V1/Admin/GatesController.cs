using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Gate;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Wallet.Interface;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Controllers.Site.V1.Admin
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Admin")]
    [ApiController]
    [ServiceFilter(typeof(DocumentApproveFilter))]
    public class GatesController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<GatesController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWalletService _walletService;
        private readonly IWebHostEnvironment _env;
        public GatesController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<GatesController> logger, IUploadService uploadService,
            IWebHostEnvironment env, IWalletService walletService)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
            _walletService = walletService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet(ApiV1Routes.AdminGates.GetGates)]
        public async Task<IActionResult> GetUserGates(string userId)
        {
            var gatesFromRepo = await _db.GateRepository
                 .GetManyAsync(p => p.Wallet.UserId == userId, null, "");

            var gates = _mapper.Map<List<GateForReturnDto>>(gatesFromRepo);

            return Ok(gates);
        }
    }
}