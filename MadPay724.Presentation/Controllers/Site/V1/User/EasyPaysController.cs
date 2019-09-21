using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.EasyPay;
using MadPay724.Data.Dtos.Site.Panel.Gate;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Data.Models.UserModel;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.User
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    [ServiceFilter(typeof(DocumentApproveFilter))]
    public class EasyPaysController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<EasyPaysController> _logger;

        public EasyPaysController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<EasyPaysController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.EasyPay.GetEasyPays)]
        public async Task<IActionResult> GetEasyPays(string userId)
        {
            var easyPaysFromRepo = await _db.EasyPayRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.DateModified), "");

            var bankcards = _mapper.Map<List<EasyPayForReturnDto>>(easyPaysFromRepo);

            return Ok(bankcards);
        }

        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.EasyPay.GetEasyPay, Name = "GetEasyPay")]
        public async Task<IActionResult> GetEasyPay(string id, string userId)
        {
            var easyPayFromRepo = await _db.EasyPayRepository.GetByIdAsync(id);
            if (easyPayFromRepo != null)
            {
                if (easyPayFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var easyPay = _mapper.Map<EasyPayForReturnDto>(easyPayFromRepo);

                    return Ok(easyPay);
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به ایزی پی دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به ایزی پی کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("ایزی پیی وجود ندارد");
            }

        }
        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.EasyPay.GetEasyPayGatesWallets)]
        public async Task<IActionResult> GetEasyPayGatesWallets(string id, string userId)
        {
            var easyPayFromRepo = await _db.EasyPayRepository.GetByIdAsync(id);
            if (easyPayFromRepo != null)
            {
                if (easyPayFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var easyPay = _mapper.Map<EasyPayForReturnDto>(easyPayFromRepo);

                    var gatesFromRepo = await _db.GateRepository
                        .GetManyAsync(p => p.Wallet.UserId == userId, s => s.OrderByDescending(x => x.IsActive), "");

                    var walletsFromRepo = await _db.WalletRepository
                        .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.IsMain).ThenByDescending(x => x.IsSms), "");

                    var result = new EasyPayGatesWalletsForReturnDto()
                    {
                        EasyPay = easyPay,
                        Gates = _mapper.Map<List<GateForReturnDto>>(gatesFromRepo),
                        Wallets = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo)
                    };

                    return Ok(result);
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به ایزی پی دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به ایزی پی کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("ایزی پیی وجود ندارد");
            }

        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(ApiV1Routes.EasyPay.AddEasyPay)]
        public async Task<IActionResult> AddEasyPay(string userId, EasyPayForCreateUpdateDto easyPayForCreateDto)
        {
            var easyPayFromRepo = await _db.EasyPayRepository
                .GetAsync(p => p.Name == easyPayForCreateDto.Name && p.UserId == userId);

            if (easyPayFromRepo == null)
            {
                var cardForCreate = new EasyPay()
                {
                    UserId = userId,
                };
                if (!easyPayForCreateDto.IsCountLimit)
                {
                    easyPayForCreateDto.CountLimit = 0;
                }
                var easyPay = _mapper.Map(easyPayForCreateDto, cardForCreate);

                await _db.EasyPayRepository.InsertAsync(easyPay);

                if (await _db.SaveAsync())
                {
                    var easyPayForReturn = _mapper.Map<EasyPayForReturnDto>(easyPay);

                    return CreatedAtRoute("GetEasyPay", new { id = easyPay.Id, userId = userId }, easyPayForReturn);
                }
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            {
                return BadRequest("این ایزی پی قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpPut(ApiV1Routes.EasyPay.UpdateEasyPay)]
        public async Task<IActionResult> UpdateEasyPay(string id, string userId, EasyPayForCreateUpdateDto easyPayForUpdateDto)
        {

            var epFromRepo = await _db.EasyPayRepository
              .GetAsync(p => p.Name == easyPayForUpdateDto.Name && p.UserId == userId && p.Id != id);
            if (epFromRepo == null)
            {
                var easyPayFromRepo = await _db.EasyPayRepository.GetByIdAsync(id);
                if (easyPayFromRepo != null)
                {
                    if (easyPayFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    {
                        var easyPay = _mapper.Map(easyPayForUpdateDto, easyPayFromRepo);
                        easyPay.DateModified = DateTime.Now;
                        if (!easyPayForUpdateDto.IsCountLimit)
                        {
                            easyPayForUpdateDto.CountLimit = 0;
                        }
                        _db.EasyPayRepository.Update(easyPay);

                        if (await _db.SaveAsync())
                            return NoContent();
                        else
                            return BadRequest("خطا در ثبت اطلاعات");
                    }
                    else
                    {
                        _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد اپدیت به ایزی پی دیگری را دارد");

                        return BadRequest("شما اجازه اپدیت ایزی پی کاربر دیگری را ندارید");
                    }
                }
                {
                    return BadRequest("ایزی پیی وجود ندارد");
                }
            }
            {
                return BadRequest("این ایزی پی قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpDelete(ApiV1Routes.EasyPay.DeleteEasyPay)]
        public async Task<IActionResult> DeleteEasyPay(string id, string userId)
        {
            var easyPayFromRepo = await _db.EasyPayRepository.GetByIdAsync(id);
            if (easyPayFromRepo != null)
            {
                if (easyPayFromRepo.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    _db.EasyPayRepository.Delete(easyPayFromRepo);

                    if (await _db.SaveAsync())
                        return NoContent();
                    else
                        return BadRequest("خطا در حذف اطلاعات");
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد حذف به ایزی پی دیگری را دارد");

                    return BadRequest("شما اجازه حذف ایزی پی کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("ایزی پیی وجود ندارد");
            }
        }
    }
}