
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Enums;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Wallet.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Panel.Admin
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Admin")]
    [ApiController]
    public class FactorsController : ControllerBase
    {
        private readonly IUnitOfWork<Financial_MadPayDbContext> _db;
        private readonly IUnitOfWork<Main_MadPayDbContext> _dbMain;
        private readonly IMapper _mapper;
        private readonly ILogger<FactorsController> _logger;
        private readonly IWalletService _walletService;


        public FactorsController(IUnitOfWork<Financial_MadPayDbContext> db, IMapper mapper,
            ILogger<FactorsController> logger, IWalletService walletService, IUnitOfWork<Main_MadPayDbContext> dbMain)
        {
            _dbMain = dbMain;
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _walletService = walletService;
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.AdminFactors.GetUserFactors)]
        public async Task<IActionResult> GetUserFactors(string userId,[FromQuery]FactorPaginationDto factorPaginationDto)
        {
            var factorsFromRepo = await _db.FactorRepository
                    .GetAllPagedListAsync(
                    factorPaginationDto,
                    factorPaginationDto.ToFactorExpression(SearchIdEnums.User, userId),
                    factorPaginationDto.SortHe.ToOrderBy(factorPaginationDto.SortDir),
                    "");//,Factors

            Response.AddPagination(factorsFromRepo.CurrentPage, factorsFromRepo.PageSize,
                factorsFromRepo.TotalCount, factorsFromRepo.TotalPage);

            return Ok(factorsFromRepo);
        }


    }
}