using System;
using System.Threading.Tasks;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using MadPay724.Data.Dtos.Site.Admin.Users;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Presentation.Routes.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.V1.Site.Admin
{
    [AllowAnonymous]
    [ApiExplorerSettings(GroupName = "v1_Site_Admin")]
    //[Route("api/v1/site/admin/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly IUtilities _utilities;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public AuthController(IUnitOfWork<MadpayDbContext> dbContext, IAuthService authService,
            IConfiguration config, IMapper mapper, ILogger<AuthController> logger, IUtilities utilities,
            UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _db = dbContext;
            _authService = authService;
            _config = config;
            _mapper = mapper;
            _logger = logger;
            _utilities = utilities;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpPost(ApiV1Routes.Auth.Register)]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {



            //if (await _db.UserRepository.UserExists(userForRegisterDto.UserName))
            //{
            //    _logger.LogWarning($"{userForRegisterDto.Name} - {userForRegisterDto.UserName} میحواهد دوباره ثبت نام کند");
            //    return BadRequest(new ReturnMessage()
            //    {
            //        status = false,
            //        title = "خطا",
            //        message = "نام کاربری وجود دارد"
            //    });
            //}


            var userToCreate = new User
            {
                UserName = userForRegisterDto.UserName,
                Name = userForRegisterDto.Name,
                PhoneNumber = userForRegisterDto.PhoneNumber,
                Address = "",
                City = "",
                Gender = true,
                DateOfBirth = DateTime.Now,
                IsAcive = true,
                Status = true,
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

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            if (result.Succeeded)
            {
                await _authService.AddUserPhotos(photoToCreate);

                var userForReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

                _logger.LogInformation($"{userForRegisterDto.Name} - {userForRegisterDto.UserName} ثبت نام کرده است");

                return CreatedAtRoute("GetUser", new
                {
                    controller = "Users",
                    id = userToCreate.Id
                }, userForReturn);
            }
            else if(result.Errors.Any())
            {
                _logger.LogWarning(result.Errors.First().Description);
                return BadRequest(new ReturnMessage()
                {
                    status = false,
                    title = "خطا",
                    message = result.Errors.First().Description
                });
            }
            else
            {
                return BadRequest(new ReturnMessage()
                {
                    status = false,
                    title = "خطا",
                    message ="خطای نامشخص"
                });
            }


        }
        [HttpPost(ApiV1Routes.Auth.Login)]
        public async Task<IActionResult> Login(UseForLoginDto useForLoginDto)
        {
            var user = await _userManager.FindByNameAsync(useForLoginDto.UserName);
            if (user == null)
            {
                _logger.LogWarning($"{useForLoginDto.UserName} درخواست لاگین ناموفق داشته است");
                return Unauthorized("کاربری با این یوزر و پس وجود ندارد");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, useForLoginDto.Password, false);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.Include(p => p.Photos)
                    .FirstOrDefault(u => u.NormalizedUserName == useForLoginDto.UserName.ToUpper());

                var userForReturn = _mapper.Map<UserForDetailedDto>(appUser);

                _logger.LogInformation($"{useForLoginDto.UserName} لاگین کرده است");
                return Ok(new
                {
                    token = _utilities.GenerateJwtToken(appUser, useForLoginDto.IsRemember),
                    user = userForReturn
                });
            }
            else
            {
                _logger.LogWarning($"{useForLoginDto.UserName} درخواست لاگین ناموفق داشته است");
                return Unauthorized("کاربری با این یوزر و پس وجود ندارد");

            }
        }
    }
}