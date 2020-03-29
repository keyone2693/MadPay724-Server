using AutoMapper;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Gate;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Data.Models.MainDB.UserModel;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Common.Routes.V1.Site;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Wallet.Interface;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MadPay724.Common.Helpers.Interface;

namespace MadPay724.Presentation.Controllers.V1.Panel.User
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
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
        private readonly IUtilities _utilities;
        public GatesController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<GatesController> logger, IUploadService uploadService,
            IWebHostEnvironment env, IWalletService walletService, IUtilities utilities)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
            _walletService = walletService;
            _utilities = utilities;
        }


        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(SiteV1Routes.Gate.GetGates)]
        public async Task<IActionResult> GetGates(string userId)
        {
            var gatesFromRepo = await _db.GateRepository
                .GetManyAsync(p => p.Wallet.UserId == userId, s => s.OrderByDescending(x => x.IsActive), "");

            var walletsFromRepo = await _db.WalletRepository
                .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.IsMain).ThenByDescending(x => x.IsSms), "");

            var result = new GatesWalletsForReturnDto()
            {
                Gates = _mapper.Map<List<GateForReturnDto>>(gatesFromRepo),
                Wallets = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo)
            };

            return Ok(result);
        }
        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(SiteV1Routes.Gate.GetGate, Name = "GetGate")]
        public async Task<IActionResult> GetGate(string id, string userId)
        {

            var gateFromRepo = (await _db.GateRepository
                .GetManyAsync(p => p.Id == id, null, "Wallet")).SingleOrDefault();

            if (gateFromRepo != null)
            {
                if (gateFromRepo.Wallet.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    var walletsFromRepo = await _db.WalletRepository
                        .GetManyAsync(p => p.UserId == userId, s => s.OrderByDescending(x => x.IsMain).ThenByDescending(x => x.IsSms), "");

                    var result = new GateWalletsForReturnDto()
                    {
                        Gate = _mapper.Map<GateForReturnDto>(gateFromRepo),
                        Wallets = _mapper.Map<List<WalletForReturnDto>>(walletsFromRepo)
                    };

                    return Ok(result);
                }
                else
                {
                    _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد دسترسی به درگاه دیگری را دارد");

                    return BadRequest("شما اجازه دسترسی به درگاه کاربر دیگری را ندارید");
                }
            }
            else
            {
                return BadRequest("درگاهی وجود ندارد");
            }

        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPost(SiteV1Routes.Gate.AddGate)]
        public async Task<IActionResult> AddGate(string userId, [FromForm]GateForCreateDto gateForCreateDto)
        {
            var gateFromRepo = await _db.GateRepository
                .GetAsync(p => p.WebsiteUrl == gateForCreateDto.WebsiteUrl && p.Wallet.UserId == userId);

            if (gateFromRepo == null)
            {
                var gateForCreate = new Gate()
                {
                    WalletId = gateForCreateDto.WalletId,
                    IsDirect = false,
                    IsActive = false,
                    Ip =await _utilities.GetDomainIpAsync(gateForCreateDto.WebsiteUrl)
                };
                if (gateForCreateDto.File != null)
                {
                    if (gateForCreateDto.File.Length > 0)
                    {
                        var uploadRes = await _uploadService.UploadFileToLocal(
                            gateForCreateDto.File,
                            Guid.NewGuid().ToString(),
                            _env.WebRootPath,
                            $"{Request.Scheme ?? ""}://{Request.Host.Value ?? ""}{Request.PathBase.Value ?? ""}",
                            "Files\\Gate"
                        );
                        if (uploadRes.Status)
                        {
                            gateForCreate.IconUrl = uploadRes.Url;
                        }
                        else
                        {
                            return BadRequest(uploadRes.Message);
                        }
                    }
                    else
                    {
                        gateForCreate.IconUrl = string.Format("{0}://{1}{2}/{3}",
                          Request.Scheme,
                          Request.Host.Value ?? "",
                          Request.PathBase.Value ?? "",
                          "wwwroot/Files/Pic/Logo/logo-gate.png");
                    }
                }
                else
                {
                    gateForCreate.IconUrl = string.Format("{0}://{1}{2}/{3}",
                        Request.Scheme,
                        Request.Host.Value ?? "",
                        Request.PathBase.Value ?? "",
                        "wwwroot/Files/Pic/Logo/logo-gate.png");
                }
                var gate = _mapper.Map(gateForCreateDto, gateForCreate);

                await _db.GateRepository.InsertAsync(gate);

                if (await _db.SaveAsync())
                {
                    var gateForReturn = _mapper.Map<GateForReturnDto>(gate);

                    return CreatedAtRoute("GetGate", new { v = HttpContext.GetRequestedApiVersion().ToString(), id = gate.Id, userId = userId }, gateForReturn);
                }
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            {
                return BadRequest("این درگاه قبلا ثبت شده است");
            }


        }
        [Authorize(Policy = "RequireUserRole")]
        [HttpPut(SiteV1Routes.Gate.UpdateGate)]
        public async Task<IActionResult> UpdateGate(string userId, string id, [FromForm]GateForCreateDto gateForUpdateDto)
        {
            var gateFromRepo = (await _db.GateRepository
                .GetManyAsync(p => p.Id == id, null, "Wallet")).SingleOrDefault();

            if (gateFromRepo == null)
            {
                return BadRequest("درگاهی وجود ندارد");
            }

            if (gateFromRepo.Wallet.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                if (gateForUpdateDto.File != null)
                {
                    if (gateForUpdateDto.File.Length > 0)
                    {
                        var uploadRes = await _uploadService.UploadFileToLocal(
                            gateForUpdateDto.File,
                            gateFromRepo.Wallet.UserId,
                            _env.WebRootPath,
                            $"{Request.Scheme ?? ""}://{Request.Host.Value ?? ""}{Request.PathBase.Value ?? ""}",
                            "Files\\Gate"
                        );
                        if (uploadRes.Status)
                        {
                            gateFromRepo.IconUrl = uploadRes.Url;
                        }
                        else
                        {
                            return BadRequest(uploadRes.Message);
                        }
                    }
                }
                var gate = _mapper.Map(gateForUpdateDto, gateFromRepo);
                //
                gate.Ip = await _utilities.GetDomainIpAsync(gate.WebsiteUrl);

                _db.GateRepository.Update(gate);

                if (await _db.SaveAsync())
                    return NoContent();
                else
                    return BadRequest("خطا در ثبت اطلاعات");
            }
            else
            {
                _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد اپدیت به درگاه دیگری را دارد");

                return BadRequest("شما اجازه اپدیت درگاه کاربر دیگری را ندارید");
            }
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpPut(SiteV1Routes.Gate.ActiveDirectGate)]
        public async Task<IActionResult> ActiveDirectGate(string userId, string id, ActiveDirectGateDto activeDirectGateDto)
        {
            var gateFromRepo = (await _db.GateRepository
                .GetManyAsync(p => p.Id == id, null, "Wallet")).SingleOrDefault();

            if (gateFromRepo == null)
            {
                return BadRequest("درگاهی وجود ندارد");
            }
            if (!gateFromRepo.IsActive)
            {
                return BadRequest("درگاه فعال نمیباشد");
            }
            if (gateFromRepo.Wallet.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                if (activeDirectGateDto.IsDirect)
                {
                    if (await _walletService.CheckInventoryAsync(20000, activeDirectGateDto.WalletId))
                    {
                        var decResult = await _walletService.DecreaseInventoryAsync(20000, activeDirectGateDto.WalletId);
                        if (decResult.status)
                        {
                            gateFromRepo.IsDirect = activeDirectGateDto.IsDirect;
                            _db.GateRepository.Update(gateFromRepo);

                            if (await _db.SaveAsync())
                            {
                                return NoContent();
                            }
                            else
                            {
                                var incResult = await _walletService.IncreaseInventoryAsync(20000, activeDirectGateDto.WalletId);
                                if (incResult.status)
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
                    else
                    {
                        return BadRequest("کیف پول انتخابی موجودی کافی ندارد");
                    }
                }
                else
                {
                    gateFromRepo.IsDirect = activeDirectGateDto.IsDirect;
                    _db.GateRepository.Update(gateFromRepo);

                    if (await _db.SaveAsync())
                    {
                        return NoContent();
                    }
                    else
                    {
                        return BadRequest("خطا در ثبت اطلاعات");
                    }
                }
            }
            else
            {
                _logger.LogError($"کاربر   {RouteData.Values["userId"]} قصد اپدیت به درگاه دیگری را دارد");

                return BadRequest("شما اجازه اپدیت درگاه کاربر دیگری را ندارید");
            }
        }
    }
}