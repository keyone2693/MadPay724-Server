using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Common.Routes.V1.Api;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Api;
using MadPay724.Data.Dtos.Api.Pay;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly IMapper _mapper;
        private readonly ILogger<PayController> _logger;
        private readonly IUtilities _utilities;
        private GateApiReturn<string> errorModel;

        public PayController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<PayController> logger, IUtilities utilities)
        {
            _db = dbContext;
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
            var gateFromRepo = await _db.GateRepository.GetByIdAsync(payRequestDto.Api);
            if (gateFromRepo == null)
            {
                errorModel.Messages.Clear();
                errorModel.Messages = new string[] { "Api درگاه معتبر نمیباشد" };
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
            if(gateFromRepo.IsDirect)
            {

            }
            else
            {

            }
        }
    }
}