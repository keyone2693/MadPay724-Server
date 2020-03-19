using System;
using System.Threading.Tasks;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Auth.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using MadPay724.Data.Dtos.Site.Panel.Users;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.Dtos.Common.Token;
using MadPay724.Data.Dtos.Site.Panel.Auth;
using MadPay724.Presentation.Routes.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using MadPay724.Data.Dtos.Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using MadPay724.Common.Helpers.Utilities.Extensions;
using MadPay724.Services.Common;

namespace MadPay724.Presentation.Controllers.Site.V1.Auth
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}")]
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v1_Site_Panel")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly IUtilities _utilities;
        private readonly UserManager<Data.Models.MainDB.User> _userManager;
        //private readonly SignInManager<Data.Models.User> _signInManager;
        private readonly ISmsService _smsService;
        private ApiReturn<string> errorModel;


        public AuthController(IUnitOfWork<Main_MadPayDbContext> dbContext, IAuthService authService,
            IConfiguration config, IMapper mapper, ILogger<AuthController> logger, IUtilities utilities,
            UserManager<Data.Models.MainDB.User> userManager, ISmsService smsService)
        {
            _db = dbContext;
            _authService = authService;
            _config = config;
            _mapper = mapper;
            _logger = logger;
            _utilities = utilities;
            _userManager = userManager;
            _smsService = smsService;
            errorModel = new ApiReturn<string>
            {
                Status = false,
                Message = "",
                Result = null
            };
        }

        [HttpPost(ApiV1Routes.Auth.GetVerificationCode)]
        [ProducesResponseType(typeof(ApiReturn<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiReturn<int>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetVerificationCode(GetVerificationCodeDto getVerificationCodeDto)
        {
            var model = new ApiReturn<int>
            {
                Result = 0
            };

            foreach (var key in HttpContext.Session.Keys)
            {
                var ses = HttpContext.Session.GetString(key);
                if (!string.IsNullOrEmpty(ses))
                {
                    try
                    {
                        var sesRes = JsonConvert.DeserializeObject<VerificationCodeDto>(ses);
                        if (sesRes != null)
                        {
                            if (sesRes.RemoveDate < DateTime.Now)
                            {
                                HttpContext.Session.Remove(key);
                            }
                        }
                    }catch{}
                }
            }

            var oldOTP = HttpContext.Session.GetString(getVerificationCodeDto.Mobile + "-OTP");
            if (!string.IsNullOrEmpty(oldOTP))
            {
                var oldOTPRes = JsonConvert.DeserializeObject<VerificationCodeDto>(oldOTP);
                if (oldOTPRes.ExpirationDate > DateTime.Now)
                {
                    var seconds = Math.Abs((DateTime.Now - oldOTPRes.ExpirationDate).Seconds);
                    model.Status = false;
                    model.Message = "لطفا " + seconds + " ثانیه دیگر دوباره امتحان کنید ";
                    model.Result = seconds;
                    return BadRequest(model);
                }
                else
                {
                    HttpContext.Session.Remove(getVerificationCodeDto.Mobile + "-OTP");
                }
            }
            //
            getVerificationCodeDto.Mobile = getVerificationCodeDto.Mobile.ToMobile();
            if (getVerificationCodeDto.Mobile == null)
            {
                model.Status = false;
                model.Message = "شماره موبایل صحیح نمیباشد مثال : 09121234567";
                return BadRequest(model);
            }
            //
            var user = await _db.UserRepository.GetAsync(p => p.UserName == getVerificationCodeDto.Mobile);
            if (user == null)
            {
                var randomOTP = new Random().Next(10000, 99999);
                if (_smsService.SendVerificationCode(randomOTP.ToString(), getVerificationCodeDto.Mobile))
                {
                    var vc = new VerificationCodeDto
                    {
                        Code = randomOTP.ToString(),
                        ExpirationDate = DateTime.Now.AddSeconds(60),
                        RemoveDate = DateTime.Now.AddMinutes(2)
                    };
                    HttpContext.Session.SetString(getVerificationCodeDto.Mobile + "-OTP", JsonConvert.SerializeObject(vc));

                    model.Status = true;
                    model.Message = "کد فعال سازی با موفقیت ارسال شد";
                    model.Result = (int)(vc.ExpirationDate - DateTime.Now).TotalSeconds;
                    return Ok(model);
                }
                else
                {
                    model.Status = false;
                    model.Message = "خطا در ارسال کد فعال سازی";
                    return BadRequest(model);
                }
            }
            else
            {
                model.Status = false;
                model.Message = "کاربری با این شماره موبایل از قبل وجود دارد";
                return BadRequest(model);
            }
        }

        [HttpPost(ApiV1Routes.Auth.Register)]
        [ProducesResponseType(typeof(ApiReturn<UserForDetailedDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiReturn<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            var model = new ApiReturn<UserForDetailedDto>
            {
                Status = true
            };

            userForRegisterDto.UserName = userForRegisterDto.UserName.ToMobile();
            var code = HttpContext.Session.GetString(userForRegisterDto.UserName + "-OTP");
            if (string.IsNullOrEmpty(code))
            {
                errorModel.Message = "کد فعالسازی صحیح نمباشد اقدام به ارسال دوباره ی کد بکنید";
                return BadRequest(errorModel);
            }
            var codeRes = JsonConvert.DeserializeObject<VerificationCodeDto>(code);
            if (codeRes.ExpirationDate < DateTime.Now)
            {
                HttpContext.Session.Remove(userForRegisterDto.UserName + "-OTP");
                errorModel.Message = "کد فعالسازی منقضی شده است اقدام به ارسال دوباره ی کد بکنید";
                return BadRequest(errorModel);
            }
            if (codeRes.Code == userForRegisterDto.Code)
            {
                var userToCreate = new Data.Models.MainDB.User
                {
                    UserName = userForRegisterDto.UserName,
                    Name = userForRegisterDto.Name,
                    PhoneNumber = userForRegisterDto.UserName,
                    Address = "",
                    City = "",
                    Gender = true,
                    DateOfBirth = DateTime.Now,
                    IsActive = true,
                    Status = true,
                    PhoneNumberConfirmed = true
                };
                var photoToCreate = new Photo
                {
                    UserId = userToCreate.Id,
                    Url = string.Format("{0}://{1}{2}/{3}",
                        Request.Scheme,
                        Request.Host.Value ?? "",
                        Request.PathBase.Value ?? "",
                        "wwwroot/Files/Pic/profilepic.png"), //"https://res.cloudinary.com/keyone2693/image/upload/v1561717720/768px-Circle-icons-profile.svg.png",
                    Description = "Profile Pic",
                    Alt = "Profile Pic",
                    IsMain = true,
                    PublicId = "0"
                };
                var notifyToCreate = new Notification
                {
                    UserId = userToCreate.Id,
                    EnterEmail = true,
                    EnterSms = false,
                    EnterTelegram = true,
                    ExitEmail = true,
                    ExitSms = false,
                    ExitTelegram = true,
                    LoginEmail = true,
                    LoginSms = false,
                    LoginTelegram = true,
                    TicketEmail = true,
                    TicketSms = false,
                    TicketTelegram = true
                };
                var walletMain = new Wallet
                {
                    Name = "اصلی ماد پی",
                    IsMain = true,
                    IsSms = false,
                    Inventory = 0,
                    InterMoney = 0,
                    ExitMoney = 0,
                    OnExitMoney = 0

                };
                var walletSms = new Wallet
                {
                    Name = "پیامک",
                    IsMain = false,
                    IsSms = true,
                    Inventory = 0,
                    InterMoney = 0,
                    ExitMoney = 0,
                    OnExitMoney = 0

                };

                var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

                if (result.Succeeded)
                {
                    await _authService.AddUserPreNeededAsync(photoToCreate, notifyToCreate, walletMain, walletSms);

                    var userForReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

                    _logger.LogInformation($"{userForRegisterDto.Name} - {userForRegisterDto.UserName} ثبت نام کرده است");
                    //
                    model.Message = "ثبت نام شما با موفقیت انجام شد";
                    model.Result = userForReturn;
                    return CreatedAtRoute("GetUser", new
                    {
                        controller = "Users",
                        v = HttpContext.GetRequestedApiVersion().ToString(),
                        id = userToCreate.Id
                    }, model);
                }
                else if (result.Errors.Any())
                {
                    _logger.LogWarning(result.Errors.First().Description);
                    //
                    errorModel.Message = result.Errors.First().Description;
                    return BadRequest(errorModel);
                }
                else
                {
                    errorModel.Message = "خطای نامشخص";
                    return BadRequest(errorModel);
                }
            }
            else
            {
                errorModel.Message = "کد فعالسازی صحیح نمباشد اقدام به ارسال دوباره ی کد بکنید";
                return BadRequest(errorModel);
            }
        }
        [HttpPost(ApiV1Routes.Auth.Login)]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(TokenRequestDto tokenRequestDto)
        {
            switch (tokenRequestDto.GrantType)
            {
                case "password":
                    var result = await _utilities.GenerateNewTokenAsync(tokenRequestDto);
                    if (result.status)
                    {
                        var userForReturn = _mapper.Map<UserForDetailedDto>(result.user);

                        return Ok(new LoginResponseDto
                        {
                            token = result.token,
                            refresh_token = result.refresh_token,
                            user = userForReturn
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"{tokenRequestDto.UserName} درخواست لاگین ناموفق داشته است" + "---" + result.message);
                        return Unauthorized("1x111keyvanx11");
                    }
                case "refresh_token":
                    var res = await _utilities.RefreshAccessTokenAsync(tokenRequestDto);
                    if (res.status)
                    {
                        return Ok(res);
                    }
                    else
                    {
                        _logger.LogWarning($"{tokenRequestDto.UserName} درخواست لاگین ناموفق داشته است" + "---" + res.message);
                        return Unauthorized("0x000keyvanx00");
                    }
                default:
                    return Unauthorized("خطا در اعتبار سنجی دوباره");
            }
        }
    }
}