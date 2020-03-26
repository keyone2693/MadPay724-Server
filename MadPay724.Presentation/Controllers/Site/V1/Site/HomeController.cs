using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Site
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Accountant")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IMapper _mapper;
        private readonly ILogger<HomeController> _logger;
        private readonly IUtilities _utilities;
        //private readonly SignInManager<Data.Models.User> _signInManager;
        private ApiReturn<string> errorModel;


        public HomeController(IUnitOfWork<Main_MadPayDbContext> dbContext,
            IMapper mapper, ILogger<HomeController> logger, IUtilities utilities)
        {
            _db = dbContext;
            _mapper = mapper;
            _logger = logger;
            _utilities = utilities;
            errorModel = new ApiReturn<string>
            {
                Status = false,
                Message = "",
                Result = null
            };
        }

        [HttpPost(ApiV1Routes.Home.GetHomeData)]
        [ProducesResponseType(typeof(ApiReturn<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiReturn<int>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHomeData()
        {
            var model = new ApiReturn<int>
            {
                Result = 0
            };
            if ()
            {

            }
            else
            {
                model.Status = false;
                model.Message = "";
                return BadRequest(model);
            }
        }
    }
}