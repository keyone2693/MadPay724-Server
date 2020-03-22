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
            getVerificationCodeDto.Mobile = getVerificationCodeDto.Mobile.ToMobile();
            if (getVerificationCodeDto.Mobile == null)
            {
                model.Status = false;
                model.Message = "شماره موبایل صحیح نمیباشد مثال : 09121234567";
                return BadRequest(model);
            }
            var OtpId = getVerificationCodeDto.Mobile + "-OTP";

            var verfyCodes = await _db.VerificationCodeRepository.GetAllAsync();
            foreach (var vc in verfyCodes.Where(p => p.RemoveDate < DateTime.Now))
            {
                if (vc.RemoveDate < DateTime.Now)
                {
                    _db.VerificationCodeRepository.Delete(vc.Id);
                }
                await _db.SaveAsync();
            }

            var oldOTP = verfyCodes.SingleOrDefault(p => p.Id == OtpId);
            if (oldOTP != null)
            {
                if (oldOTP.ExpirationDate > DateTime.Now)
                {
                    var seconds = Math.Abs((DateTime.Now - oldOTP.ExpirationDate).Seconds);
                    model.Status = false;
                    model.Message = "لطفا " + seconds + " ثانیه دیگر دوباره امتحان کنید ";
                    model.Result = seconds;
                    return BadRequest(model);
                }
                else
                {
                    _db.VerificationCodeRepository.Delete(OtpId);
                    await _db.SaveAsync();
                }
            }
            //
            var user = await _db.UserRepository.GetAsync(p => p.UserName == getVerificationCodeDto.Mobile);
            if (user == null)
            {
                var randomOTP = new Random().Next(10000, 99999);
                if (_smsService.SendVerificationCode(getVerificationCodeDto.Mobile, randomOTP.ToString()))
                {
                    var vc = new VerificationCode
                    {
                        Code = randomOTP.ToString(),
                        ExpirationDate = DateTime.Now.AddSeconds(60),
                        RemoveDate = DateTime.Now.AddMinutes(2)
                    };
                    vc.Id = OtpId;
                    //
                    await _db.VerificationCodeRepository.InsertAsync(vc);
                    await _db.SaveAsync();

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
            if (userForRegisterDto.UserName == null)
            {
                model.Status = false;
                model.Message = "شماره موبایل صحیح نمیباشد مثال : 09121234567";
                return BadRequest(model);
            }
            var OtpId = userForRegisterDto.UserName + "-OTP";
            //
            var code = await _db.VerificationCodeRepository.GetByIdAsync(OtpId);
            if (code == null)
            {
                errorModel.Message = "کد فعالسازی صحیح نمباشد اقدام به ارسال دوباره ی کد بکنید";
                return BadRequest(errorModel);
            }
            if (code.ExpirationDate < DateTime.Now)
            {
                _db.VerificationCodeRepository.Delete(OtpId);
                await _db.SaveAsync();
                errorModel.Message = "کد فعالسازی منقضی شده است اقدام به ارسال دوباره ی کد بکنید";
                return BadRequest(errorModel);
            }
            if (code.Code == userForRegisterDto.Code)
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
                    UserId = userToCreate.Id,
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
                    UserId = userToCreate.Id,
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

                    var creaatedUser = await _userManager.FindByNameAsync(userToCreate.UserName);
                    await _userManager.AddToRolesAsync(creaatedUser, new[] { "User" });

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
        [HttpPost(ApiV1Routes.Auth.RegisterWithSocial)]
        [ProducesResponseType(typeof(ApiReturn<UserForDetailedDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiReturn<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterWithSocial(UserForRegisterWithSocialDto userForRegisterWithSocialDto)
        {
            var model = new ApiReturn<UserForDetailedDto>
            {
                Status = true
            };

            var user = await _db.UserRepository.GetByIdAsync(userForRegisterWithSocialDto.UserId);
            if (user != null)
            {
                var oldphoto = await _db.PhotoRepository.GetAsync(p => p.UserId == userForRegisterWithSocialDto.UserId && p.IsMain);
                oldphoto.Url = userForRegisterWithSocialDto.PhotoUrl;
                _db.PhotoRepository.Update(oldphoto);
                await _db.SaveAsync();

                model.Message = "ورود شما با موفقیت انجام شد";
                model.Result = _mapper.Map<UserForDetailedDto>(user);
                model.Result.IsRegisterBefore = true;
                return CreatedAtRoute("GetUser", new
                {
                    controller = "Users",
                    v = HttpContext.GetRequestedApiVersion().ToString(),
                    id = userForRegisterWithSocialDto.UserId
                }, model);
            }
            else
            {
                var userToCreate = new Data.Models.MainDB.User
                {
                    UserName = userForRegisterWithSocialDto.Email,
                    Name = userForRegisterWithSocialDto.Name,
                    PhoneNumber = "0000",
                    Address = "",
                    City = "",
                    Gender = true,
                    DateOfBirth = DateTime.Now,
                    IsActive = true,
                    Status = true,
                    PhoneNumberConfirmed = true
                };
                userToCreate.Id = userForRegisterWithSocialDto.UserId;
                var photoToCreate = new Photo
                {
                    UserId = userForRegisterWithSocialDto.UserId,
                    Url = userForRegisterWithSocialDto.PhotoUrl,
                    Description = "Profile Pic",
                    Alt = "Profile Pic",
                    IsMain = true,
                    PublicId = "2"
                };
                var notifyToCreate = new Notification
                {
                    UserId = userForRegisterWithSocialDto.UserId,
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
                    UserId = userForRegisterWithSocialDto.UserId,
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
                    UserId = userForRegisterWithSocialDto.UserId,
                    Name = "پیامک",
                    IsMain = false,
                    IsSms = true,
                    Inventory = 0,
                    InterMoney = 0,
                    ExitMoney = 0,
                    OnExitMoney = 0

                };

                var result = await _userManager.CreateAsync(userToCreate, userForRegisterWithSocialDto.Email);
                if (result.Succeeded)
                {
                    await _authService.AddUserPreNeededAsync(photoToCreate, notifyToCreate, walletMain, walletSms);
                    var creaatedUser = await _userManager.FindByNameAsync(userToCreate.UserName);
                    await _userManager.AddToRolesAsync(creaatedUser, new[] { "User" });
                    var userForReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
                    userForReturn.IsRegisterBefore = false;
                    _logger.LogInformation($"{userForRegisterWithSocialDto.Name} - {userForRegisterWithSocialDto.Email} ثبت نام کرده است");
                    //
                    model.Message = "ورود شما با موفقیت انجام شد";
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


        }

        [HttpPost(ApiV1Routes.Auth.Login)]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(TokenRequestDto tokenRequestDto)
        {
            switch (tokenRequestDto.GrantType)
            {
                
                case "password":
                    var result = await _utilities.GenerateNewTokenAsync(tokenRequestDto,true);
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
                case "social":
                    var socialresult = await _utilities.GenerateNewTokenAsync(tokenRequestDto,false);
                    if (socialresult.status)
                    {
                        var userForReturn = _mapper.Map<UserForDetailedDto>(socialresult.user);

                        return Ok(new LoginResponseDto
                        {
                            token = socialresult.token,
                            refresh_token = socialresult.refresh_token,
                            user = userForReturn
                        });
                    }
                    else
                    {
                        _logger.LogWarning($"{tokenRequestDto.UserName} درخواست لاگین ناموفق داشته است" + "---" + socialresult.message);
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