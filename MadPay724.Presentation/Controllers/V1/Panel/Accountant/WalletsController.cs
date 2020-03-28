using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.BankCards;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Common.Routes.V1.Site;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace MadPay724.Presentation.Controllers.V1.Panel.Accountant
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<WalletsController> _logger;



        public WalletsController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IMapper mapper,
            ILogger<WalletsController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(SiteV1Routes.Accountant.GetWallets)]
        public async Task<IActionResult> GetWallets([FromQuery]PaginationDto paginationDto)
        {
            var walletsFromRepo = await _db.WalletRepository
                 .GetAllPagedListAsync(
                 paginationDto,
                 paginationDto.Filter.ToWalletExpression(true),
                 paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                 "");

            Response.AddPagination(walletsFromRepo.CurrentPage, walletsFromRepo.PageSize,
                walletsFromRepo.TotalCount, walletsFromRepo.TotalPage);

            var wallets = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo);


            return Ok(wallets);
        }

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(SiteV1Routes.Accountant.GetInventoryWallets)]
        public async Task<IActionResult> GetUserWallets(string userId)
        {

            var walletsFromRepo = await _db.WalletRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.IsMain).ThenByDescending(x => x.IsSms), "");

            var wallets = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo);

            return Ok(wallets);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(SiteV1Routes.Accountant.BlockInventoryWallet)]
        public async Task<IActionResult> BlockWallet(string walletId, WalletBlockDto walletBlockDto)
        {
            var walletsFromRepo = await _db.WalletRepository.GetByIdAsync(walletId);
            walletsFromRepo.IsBlock = walletBlockDto.Block;
            _db.WalletRepository.Update(walletsFromRepo);

            if (await _db.SaveAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("خطا در تغییر بلاکی بودن کیف پول");
            }
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(SiteV1Routes.Accountant.ApproveInventoryWallet)]
        public async Task<IActionResult> ApproveWallet(string bankcardId, BankCardApproveDto bankCardApproveDto)
        {
            var bankcardFromRepo = await _db.BankCardRepository.GetByIdAsync(bankcardId);
            bankcardFromRepo.Approve = bankCardApproveDto.Approve;
            _db.BankCardRepository.Update(bankcardFromRepo);

            if (await _db.SaveAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("خطا در تغییر تاییدی بودن کارت");
            }
        }

    }
}