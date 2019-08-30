using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.UserModel;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MadPay724.Presentation.Controllers.Site.V1.User
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    [ServiceFilter(typeof(DocumentApproveFilter))]
    public class GatesController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<GatesController> _logger;

        public GatesController(IUnitOfWork<MadpayDbContext> dbContext, IMapper mapper,
            ILogger<GatesController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
        }


        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Gate.GetGates)]
        public async Task<IActionResult> GetGates(string userId)
        {
            var gatesFromRepo = await _db.GateRepository
                .GetManyAsync(p => p.Wallet.UserId == userId, s => s.OrderByDescending(x => x.IsActive), "");


            var bankcards = _mapper.Map<List<GateForReturnDto>>(gatesFromRepo);

            return Ok(bankcards);
        }

        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Gate.GetGate, Name = "GetGate")]
        public async Task<IActionResult> GetGate(string id, string userId)
        {
            var gateFromRepo = (await _db.GateRepository
                .GetManyAsync(p => p.Id == id, null, "Wallets")).SingleOrDefault();

            if (gateFromRepo != null)
            {
                if (gateFromRepo.Wallet.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var gate = _mapper.Map<GateForReturnDto>(gateFromRepo);

                    return Ok(gate);
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
        [HttpPost(ApiV1Routes.Gate.AddGate)]
        public async Task<IActionResult> AddGate(string userId, GateForCreateDto gateForCreateDto)
        {
            var gateFromRepo = await _db.GateRepository
                .GetAsync(p => p.WebsiteUrl == gateForCreateDto.WebsiteUrl && p.Wallet.UserId == userId);

            if (gateFromRepo == null)
            {
                var cardForCreate = new Gate()
                {
                    WalletId = gateForCreateDto.WalletId,
                    IsDirect = false,
                    IsActive = false
                };
                var gate = _mapper.Map(gateForCreateDto, cardForCreate);

                await _db.GateRepository.InsertAsync(gate);

                if (await _db.SaveAsync())
                {
                    var gateForReturn = _mapper.Map<GateForReturnDto>(gate);

                    return CreatedAtRoute("GetGate", new { id = gate.Id, userId = userId }, gateForReturn);
                }
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            {
                return BadRequest("این کارت قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpPut(ApiV1Routes.Gate.UpdateGate)]
        public async Task<IActionResult> UpdateGate(string id, gateForCreateDto gateForUpdateDto)
        {
            var gateFromRepo =  (await _db.GateRepository
                .GetManyAsync(p => p.Id == id, null, "Wallets")).SingleOrDefault();
            if (gateFromRepo != null)
            {
                if (gateFromRepo.Wallet.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var gate = _mapper.Map(gateForUpdateDto, gateFromRepo);
                    gate.Approve = false;
                    _db.GateRepository.Update(gate);

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
    }
}