using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.BankCards;
using MadPay724.Data.Models.MainDB;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Panel.User
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    [ServiceFilter(typeof(DocumentApproveFilter))]
    public class BankCardsController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<BankCardsController> _logger;

        public BankCardsController(IUnitOfWork<Main_MadPayDbContext> dbContext
            , IUnitOfWork<Financial_MadPayDbContext> dbFinancial, IMapper mapper,
            ILogger<BankCardsController> logger)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.BankCard.GetBankCards)]
        public async Task<IActionResult> GetBankCards(string userId)
        {
            var bankCardsFromRepo = await _db.BankCardRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.Approve), "");


            var bankcards = _mapper.Map<List<BankCardForUserDetailedDto>>(bankCardsFromRepo);

            return Ok(bankcards);
        }

        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.BankCard.GetBankCard, Name = "GetBankCard")]
        public async Task<IActionResult> GetBankCard(string id, string userId)
        {
            var bankCardFromRepo = await _db.BankCardRepository.GetByIdAsync(id);
            if (bankCardFromRepo != null)
            {
                if (bankCardFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var bankCard = _mapper.Map<BankCardForReturnDto>(bankCardFromRepo);

                    return Ok(bankCard);
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به کارت دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به کارت کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("کارتی وجود ندارد");
            }

        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.BankCard.AddBankCard)]
        public async Task<IActionResult> AddBankCard(string userId, BankCardForUpdateDto bankCardForUpdateDto)
        {
            var bankCardFromRepo = await _db.BankCardRepository
                .GetAsync(p => p.CardNumber == bankCardForUpdateDto.CardNumber && p.UserId == userId);
            var bankCardCount = await _db.BankCardRepository.BankCardCountAsync(userId);

            if (bankCardFromRepo == null)
            {
                if (bankCardCount <= 10)
                {
                    var cardForCreate = new BankCard()
                    {
                        UserId = userId,
                        Approve = false
                    };
                    var bankCard = _mapper.Map(bankCardForUpdateDto, cardForCreate);

                    await _db.BankCardRepository.InsertAsync(bankCard);

                    if (await _db.SaveAsync())
                    {
                        var bankCardForReturn = _mapper.Map<BankCardForReturnDto>(bankCard);

                        return CreatedAtRoute("GetBankCard", new { v = HttpContext.GetRequestedApiVersion().ToString(), id = bankCard.Id, userId = userId }, bankCardForReturn);
                    }
                    else
                        return BadRequest("خطا در ثبت اطلاعات");
                }
                {
                    return BadRequest("شما اجازه وارد کردن بیش از 10 کارت را ندارید");
                }
            }
            {
                return BadRequest("این کارت قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpPut(ApiV1Routes.BankCard.UpdateBankCard)]
        public async Task<IActionResult> UpdateBankCard(string id, BankCardForUpdateDto bankCardForUpdateDto)
        {
            var bankCardFromRepo = await _db.BankCardRepository.GetByIdAsync(id);
            if (bankCardFromRepo != null)
            {
                if (bankCardFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var bankCard = _mapper.Map(bankCardForUpdateDto, bankCardFromRepo);
                    bankCard.Approve = false;
                    _db.BankCardRepository.Update(bankCard);

                    if (await _db.SaveAsync())
                        return NoContent();
                    else
                        return BadRequest("خطا در ثبت اطلاعات");
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد اپدیت به کارت دیگری را دارد");

                    return BadRequest("شما اجازه اپدیت کارت کاربر دیگری را ندارید");
                }
            }
            {
                return BadRequest("کارتی وجود ندارد");
            }
        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpDelete(ApiV1Routes.BankCard.DeleteBankCard)]
        public async Task<IActionResult> DeleteBankCard(string id)
        {
            //return BadRequest("شما اجازه حذف کارت هارا ندارید");

            var bankCardFromRepo = await _db.BankCardRepository.GetByIdAsync(id);
            if (bankCardFromRepo != null)
            {

                if (await _dbFinancial.EntryRepository.IsAnyAsync(p=>p.BankCardId == id))
                {
                    return BadRequest("کارتی که برای آن واریزی ثبت شده است امکان حذف ندارد");
                }

                if (bankCardFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    _db.BankCardRepository.Delete(bankCardFromRepo);

                    if (await _db.SaveAsync())
                        return NoContent();
                    else
                        return BadRequest("خطا در حذف اطلاعات");
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد حذف به کارت دیگری را دارد");

                    return BadRequest("شما اجازه حذف کارت کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("کارتی وجود ندارد");
            }
        }
    }
}