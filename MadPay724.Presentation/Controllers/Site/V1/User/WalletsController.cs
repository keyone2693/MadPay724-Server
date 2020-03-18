using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Data.Models.MainDB;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Wallet.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.User
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    [ServiceFilter(typeof(DocumentApproveFilter))]
    public class WalletsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<WalletsController> _logger;
        private readonly IWalletService _walletService;

        public WalletsController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<WalletsController> logger, IWalletService walletService)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _walletService = walletService;
        }


        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Wallet.GetWallets)]
        public async Task<IActionResult> GetWallets(string userId)
        {
            var walletsFromRepo = await _db.WalletRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.IsMain).ThenByDescending(x => x.IsSms), "");

            var wallets = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo);

            return Ok(wallets);
        }

        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Wallet.GetWallet, Name = "GetWallet")]
        public async Task<IActionResult> GetWallet(string id, string userId)
        {
            var walletFromRepo = await _db.WalletRepository.GetByIdAsync(id);
            if (walletFromRepo != null)
            {
                if (walletFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var wallet = _mapper.Map<WalletForReturnDto>(walletFromRepo);

                    return Ok(wallet);
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به کیف پول دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به کیف پول کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("کیف پولی وجود ندارد");
            }

        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.Wallet.AddWallet)]
        public async Task<IActionResult> AddWallet(string userId, WalletForCreateDto walletForCreateDto)
        {
            var walletFromRepo = await _db.WalletRepository
                .GetAsync(p => p.Name == walletForCreateDto.WalletName && p.UserId == userId);
            var walletCount = await _db.WalletRepository.WalletCountAsync(userId);

            if (walletFromRepo == null)
            {
                if (walletCount <= 10)
                {
                    var code = await _db.WalletRepository.GetLastWalletCodeAsync() +1;

                    while (await _db.WalletRepository.WalletCodeExistAsync(code))
                    {
                        code += 1;
                    }

                    if (await _walletService.CheckInventoryAsync(1500, walletForCreateDto.WalletId))
                    {
                        var decResult = await _walletService.DecreaseInventoryAsync(1500, walletForCreateDto.WalletId);
                        if (decResult.status)
                        {
                            var wallet = new Wallet()
                            {
                                UserId = userId,
                                IsBlock = false,
                                Code = code,
                                Name = walletForCreateDto.WalletName,
                                IsMain = false,
                                IsSms = false,
                                Inventory = 0,
                                InterMoney = 0,
                                ExitMoney = 0,
                                OnExitMoney = 0
                            };

                            await _db.WalletRepository.InsertAsync(wallet);

                            if (await _db.SaveAsync())
                            {
                                var walletForReturn = _mapper.Map<WalletForReturnDto>(wallet);

                                return CreatedAtRoute("GetWallet", new { v = HttpContext.GetRequestedApiVersion().ToString(), id = wallet.Id, userId = userId },
                                    walletForReturn);
                            }
                            else
                            {
                                var incResult = await _walletService.IncreaseInventoryAsync(1500, walletForCreateDto.WalletId);
                                if(incResult.status)
                                    return BadRequest("خطا در ثبت اطلاعات");
                                else
                                    return BadRequest("خطا در ثبت اطلاعات در صورت کسری موجودی با پشتیبانی در تماس باشید");

                            }
                        }
                        else
                        {
                            return BadRequest(decResult.message);
                        }
                    }
                    else {
                        return BadRequest("کیف پول انتخابی موجودی کافی ندارد");
                    }
                }
                {
                    return BadRequest("شما اجازه وارد کردن بیش از 10 کیف پول را ندارید");
                }
            }
            {
                return BadRequest("این کیف پول قبلا ثبت شده است");
            }


        }
    }
}