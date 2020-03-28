using AutoMapper;
using MadPay724.Common.Routes.V1.Api;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Api;
using MadPay724.Data.Dtos.Api.Pay;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private GateApiReturn<string> errorModel;

        public PayController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<PayController> logger)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            errorModel = new GateApiReturn<string>
            {
                Status = false,
                Result = null
            };
        }
        [HttpPost(ApiV1Routes.Pay.PaySend)]
        [ProducesResponseType(typeof(GateApiReturn<PayRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GateApiReturn<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PaySend(PayRequestDto payRequestDto)
        {
            var model = new GateApiReturn<PayRequestDto>
            {
                Status = true,
                Result = new PayRequestDto()
            };




            return Ok(model);
        }
    }
}