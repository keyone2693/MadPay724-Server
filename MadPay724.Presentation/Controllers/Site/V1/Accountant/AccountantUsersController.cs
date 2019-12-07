using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Pagination;
using MadPay724.Data.Dtos.Site.Panel.BankCards;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Accountant
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class AccountantUsersController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly MadpayDbContext _dbMad;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountantUsersController> _logger;
        private readonly UserManager<Data.Models.User> _userManager;



        public AccountantUsersController(IUnitOfWork<MadpayDbContext> dbContext, MadpayDbContext dbMad,
            IMapper mapper,
            ILogger<AccountantUsersController> logger, UserManager<Data.Models.User> userManager)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _dbMad = dbMad;
            _userManager = userManager;
        }
        //[AllowAnonymous]
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetInventories)]
        public async Task<IActionResult> GetInventories(string id, [FromQuery]PaginationDto paginationDto)
        {

            var usersFromRepo = await _db.UserRepository
                    .GetAllPagedListAsync(
                    paginationDto,
                    paginationDto.Filter.ToUserExpression(true),
                    paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                    "Wallets,Photos");//,BankCards

            Response.AddPagination(usersFromRepo.CurrentPage, usersFromRepo.PageSize,
                usersFromRepo.TotalCount, usersFromRepo.TotalPage);

            var users = new List<UserForAccountantDto>();

            foreach (var item in usersFromRepo)
            {
                users.Add(_mapper.Map<UserForAccountantDto>(item));
            }

            return Ok(users);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetInventoryWallets)]
        public async Task<IActionResult> GetInventoryWallets(string userId)
        {

            var walletsFromRepo = await _db.WalletRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.IsMain).ThenByDescending(x => x.IsSms), "");

            var wallets = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo);

            return Ok(wallets);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetInventoryBankCard)]
        public async Task<IActionResult> GetInventoryBankCard(string userId)
        {
            var bankCardsFromRepo = await _db.BankCardRepository
           .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.Approve), "");


            var bankcards = _mapper.Map<List<BankCardForUserDetailedDto>>(bankCardsFromRepo);

            return Ok(bankcards);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpPatch(ApiV1Routes.Accountant.BlockInventoryWallet)]
        public async Task<IActionResult> BlockInventoryWallet(string walletId, WalletBlockDto walletBlockDto)
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
        [HttpPatch(ApiV1Routes.Accountant.ApproveInventoryWallet)]
        public async Task<IActionResult> ApproveInventoryWallet(string bankcardId, BankCardApproveDto bankCardApproveDto)
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

        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetBankCards)]
        public async Task<IActionResult> GetBankCards([FromQuery]PaginationDto paginationDto)
        {
            var bankCardsFromRepo = await _db.BankCardRepository
                   .GetAllPagedListAsync(
                   paginationDto,
                   paginationDto.Filter.ToBankCardExpression(true),
                   paginationDto.SortHe.ToOrderBy(paginationDto.SortDir),
                   "");

            Response.AddPagination(bankCardsFromRepo.CurrentPage, bankCardsFromRepo.PageSize,
                bankCardsFromRepo.TotalCount, bankCardsFromRepo.TotalPage);

            var bankCards = _mapper.Map<List<BankCardForUserDetailedDto>>(bankCardsFromRepo);
            

            return Ok(bankCards);
        }
        [Authorize(Policy = "AccessAccounting")]
        [HttpGet(ApiV1Routes.Accountant.GetWallets)]
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
    }
}