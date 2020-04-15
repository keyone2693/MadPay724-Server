using AutoMapper;
using MadPay724.Common.Enums;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Common.Routes.V1.Api;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Api;
using MadPay724.Data.Dtos.Api.Pay;
using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MadPay724.Api.Controllers.V1
{
    [ApiVersion("1")]
    [Route("v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Api_Pay")]
    [ApiController]
    [AllowAnonymous]
    public class PayController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<PayController> _logger;
        private readonly IUtilities _utilities;
        private GateApiReturn<string> errorModel;

        public PayController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IUnitOfWork<Financial_MadPayDbContext> dbFinancial,
            IMapper mapper,
            ILogger<PayController> logger, IUtilities utilities)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
            _utilities = utilities;
            errorModel = new GateApiReturn<string>
            {
                Status = false,
                Result = null
            };
        }
        [HttpPost(ApiV1Routes.Pay.PaySend)]
        [ProducesResponseType(typeof(GateApiReturn<PayResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GateApiReturn<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PaySend(PayRequestDto payRequestDto)
        {        
            var model = new GateApiReturn<PayResponseDto>
            {
                Status = true,
                Result = new PayResponseDto()
            };
            //Error
            var gateFromRepo = (await _db.GateRepository.GetManyAsync(p => p.Id == payRequestDto.Api, null, "Wallet")).SingleOrDefault();
            if (gateFromRepo == null)
            {
                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { "Api درگاه معتبر نمیباشد" };
                return BadRequest(errorModel);
            }
            var userDocuments = await _db.DocumentRepository
                .GetManyAsync(p => p.Approve == 1 && p.UserId == gateFromRepo.Wallet.UserId, null, "");
            if (!userDocuments.Any())
            {
                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { "مدارک کاربر صاحب درگاه تکمیل نمیباشد" };
                return BadRequest(errorModel);
            }
            if (!gateFromRepo.IsActive)
            {
                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { "این درگاه غیر فعال میباشد . درصورت نیاز با پشتیبانی در تماس باید" };
                return BadRequest(errorModel);
            }
            if (gateFromRepo.IsIp)
            {
                var currentIp = HttpContext.Connection.RemoteIpAddress.ToString(); //::1
                var gateWebsiteIp = await _utilities.GetDomainIpAsync(gateFromRepo.WebsiteUrl);
                if (currentIp != gateWebsiteIp)
                {
                    errorModel.Messages.Clear();
                    errorModel.Messages = new string[] { "آی پی وبسایت درخواست دهنده پرداخت معبتر نمیباشد" };
                    return BadRequest(errorModel);
                }
            }
            //Success

            var factorToCreate = new Factor()
            {
                UserId = gateFromRepo.Wallet.UserId,
                GateId = gateFromRepo.Id,
                EnterMoneyWalletId = gateFromRepo.WalletId,

                UserName = payRequestDto.UserName,
                Mobile = payRequestDto.Mobile,
                Email = payRequestDto.Email,
                FactorNumber = payRequestDto.FactorNumber,
                Description = payRequestDto.Description,
                ValidCardNumber = payRequestDto.ValidCardNumber,
                RedirectUrl = payRequestDto.RedirectUrl,
                Status = false,
                Kind = (int)FactorTypeEnums.Factor,
                Bank = (int)BankEnums.ZarinPal,
                GiftCode = "",
                IsGifted = false,
                Price = payRequestDto.Amount,
                EndPrice = payRequestDto.Amount,
                RefBank = "پرداختی انجام نشده است",
                IsAlreadyVerified = false,
                GatewayName = "non",
                Message = "خطای نامشخص"
            };

            await _dbFinancial.FactorRepository.InsertAsync(factorToCreate);

            if (await _dbFinancial.SaveAsync())
            {
                model.Messages.Clear();
                model.Messages = new string[] { "بدون خطا" };
                model.Result.Token = factorToCreate.Id;
                model.Result.RedirectUrl = $"{Request.Scheme ?? ""}://{Request.Host.Value.Replace("api." , "pay.") ?? ""}{Request.PathBase.Value ?? ""}" +
                        "/bank/pay/" + factorToCreate.Id;
                return Ok(model);
            }
            else
            {
                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { "خطا در ثبت فاکتور" };
                return BadRequest(errorModel);
            }
        }
    }
}