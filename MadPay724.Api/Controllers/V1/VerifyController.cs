using AutoMapper;
using MadPay724.Common.Enums;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Common.Routes.V1.Api;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Api;
using MadPay724.Data.Dtos.Api.Pay;
using MadPay724.Data.Dtos.Api.Verify;
using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parbad;
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
    public class VerifyController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<VerifyController> _logger;
        private readonly IUtilities _utilities;
        private readonly IOnlinePayment _onlinePayment;
        private GateApiReturn<string> errorModel;

        public VerifyController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IUnitOfWork<Financial_MadPayDbContext> dbFinancial,
            IMapper mapper,ILogger<VerifyController> logger,
            IUtilities utilities, IOnlinePayment onlinePayment)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
            _utilities = utilities;
            _onlinePayment = onlinePayment;
            errorModel = new GateApiReturn<string>
            {
                Status = false,
                Result = null
            };
        }
        [HttpPost(ApiV1Routes.Verify.VerifySend)]
        [ProducesResponseType(typeof(GateApiReturn<VerifyResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GateApiReturn<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifySend(VerifyRequestDto verifyRequestDto)
        {
            
            var model = new GateApiReturn<VerifyResponseDto>
            {
                Status = true
            };
            //Error
            var factorFromRepo = await _dbFinancial.FactorRepository.GetByIdAsync(verifyRequestDto.Token);
            if (factorFromRepo == null)
            {
                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { "تراکنشی با این مشخصات یافت نشد" };
                return BadRequest(errorModel);
            }
            if (factorFromRepo.DateModified.AddMinutes(20) < DateTime.Now)
            {
                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { "زمان تایید تراکنش شما گذشته است" };
                return BadRequest(errorModel);
            }
            if (factorFromRepo.IsAlreadyVerified)
            {
                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { "این تراکنش قبلا بررسی شده است" };
                return BadRequest(errorModel);
            }
            var gateFromRepo = (await _db.GateRepository.GetManyAsync(p => p.Id == verifyRequestDto.Api, null, "Wallet")).SingleOrDefault();
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

            //Verify
            var trackingNumber = Convert.ToInt64(factorFromRepo.RefBank);
            var verifyResult = await _onlinePayment.VerifyAsync(trackingNumber);
            if (verifyResult.IsSucceed)
            {
                factorFromRepo.Status = true;
                factorFromRepo.IsAlreadyVerified = true;
                factorFromRepo.DateModified = DateTime.Now;
                factorFromRepo.Message = "تراکنش با موفقیت انجام شد";
                _dbFinancial.FactorRepository.Update(factorFromRepo);
                await _dbFinancial.SaveAsync();

                model.Messages.Clear();
                model.Messages = new string[] { "بدون خطا" };
                model.Result =new VerifyResponseDto
                {
                    Amount = factorFromRepo.EndPrice,
                    FactorNumber = factorFromRepo.FactorNumber,
                    RefBank = "MPC-"+ factorFromRepo.RefBank,
                    Mobile = factorFromRepo.Mobile,
                    Email = factorFromRepo.Email,
                    Description = factorFromRepo.Description,
                    CardNumber = factorFromRepo.ValidCardNumber
                };
                return Ok(model);
            }
            else
            {
                factorFromRepo.IsAlreadyVerified = true;
                factorFromRepo.DateModified = DateTime.Now;
                factorFromRepo.Message = verifyResult.Message;
                _dbFinancial.FactorRepository.Update(factorFromRepo);
                await _dbFinancial.SaveAsync();


                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { verifyResult.Message };
                return BadRequest(errorModel);
            }
        }
    }
}