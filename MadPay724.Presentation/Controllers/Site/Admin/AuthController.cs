using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin;
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
using Microsoft.AspNetCore.Authorization;
using MadPay724.Data.Dtos.Site.Admin.Users;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.Admin
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "Site")]
    [Route("site/admin/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork<MadpayDbContext> _db;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUnitOfWork<MadpayDbContext> dbContext, IAuthService authService,
            IConfiguration config, IMapper mapper, ILogger<AuthController> logger)
        {
            _db = dbContext;
            _authService = authService;
            _config = config;
            _mapper = mapper;
            _logger = logger;

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if (await _db.UserRepository.UserExists(userForRegisterDto.UserName))
            {
                _logger.LogWarning($"{userForRegisterDto.Name} - {userForRegisterDto.UserName} میحواهد دوباره ثبت نام کند");
                return BadRequest(new ReturnMessage()
                {
                    status = false,
                    title = "خطا",
                    message = "نام کاربری وجود دارد"
                });
            }


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
                Status = true
            };
            // var uri = Server.MapPath("~/Files/Pic/profilepic.png");

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

            var createdUser = await _authService.Register(userToCreate, photoToCreate, userForRegisterDto.Password);
            if (createdUser != null)
            {
                var userForReturn = _mapper.Map<UserForDetailedDto>(createdUser);

                _logger.LogInformation($"{userForRegisterDto.Name} - {userForRegisterDto.UserName} ثبت نام کرده است");

                return CreatedAtRoute("GetUser", new
                {
                    controller = "Users",
                    id = createdUser.Id
                }, userForReturn);
            }
            else
            {
                _logger.LogWarning($"{userForRegisterDto.Name} - {userForRegisterDto.UserName} میحواهد دوباره ثبت نام کند");
                return BadRequest(new ReturnMessage()
                {
                    status = false,
                    title = "خطا",
                    message = "ثبت در دیتابیس"
                });
            }

        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UseForLoginDto useForLoginDto)
        {
            var userFromRepo = await _authService.Login(useForLoginDto.UserName, useForLoginDto.Password);

            if (userFromRepo == null)
            {
                _logger.LogWarning($"{useForLoginDto.UserName} درخواست لاگین ناموفق داشته است");
                return Unauthorized("کاربری با این یوزر و پس وجود ندارد");

            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDes = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = useForLoginDto.IsRemember ? DateTime.Now.AddDays(1) : DateTime.Now.AddHours(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDes);


            var user = _mapper.Map<UserForDetailedDto>(userFromRepo);

            _logger.LogInformation($"{useForLoginDto.UserName} لاگین کرده است");
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });

        }
    }
}