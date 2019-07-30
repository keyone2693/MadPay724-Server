using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.AppSetting;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Token;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Auth
{
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly IUtilities _utilities;
        private readonly UserManager<Data.Models.User> _userManager;
        private readonly TokenSetting _tokenSetting;


        public TokenController(IUnitOfWork<MadpayDbContext> dbContext, IAuthService authService,
            IMapper mapper, ILogger<AuthController> logger, IUtilities utilities,
            UserManager<Data.Models.User> userManager, SignInManager<Data.Models.User> signInManager, TokenSetting tokenSetting)
        {
            _db = dbContext;
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
            _utilities = utilities;
            _userManager = userManager;
            _tokenSetting = tokenSetting;
        }


        [HttpPost]
        public async Task<IActionResult> Auth(TokenRequestDto tokenRequestDto)
        {
            switch (tokenRequestDto.GrantType)
            {
                case "password":
                    var result = await _utilities.GenerateNewTokenAsync(tokenRequestDto);
                    if (result.status)
                    {
                        return Ok(tokenRequestDto);
                    }
                    else
                    {
                        _logger.LogWarning($"{tokenRequestDto.UserName} درخواست لاگین ناموفق داشته است" + "---" + result.message);
                        return Unauthorized(result.message);
                    }
                case "refresh_token":
                    var res= await _utilities.RefreshAccessTokenAsync(tokenRequestDto);
                    if (res.status)
                    {
                        return Ok(tokenRequestDto);
                    }
                    else
                    {
                        _logger.LogWarning($"{tokenRequestDto.UserName} درخواست لاگین ناموفق داشته است"+ "---"+ res.message);
                        return Unauthorized(res.message);
                    }
                default:
                    return Unauthorized("خطا در اعتبار سنجی دوباره");
            }
        }


    }
}