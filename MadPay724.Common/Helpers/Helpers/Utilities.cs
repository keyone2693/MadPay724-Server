using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Common.Helpers.AppSetting;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Common.Token;
using MadPay724.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MadPay724.Common.Helpers.Helpers
{
    public class Utilities : IUtilities
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly TokenSetting _tokenSetting;
        private readonly MadpayDbContext _db;
        public Utilities(MadpayDbContext dbContext, IConfiguration config, UserManager<User> userManager, TokenSetting tokenSetting)
        {
            _db = dbContext;
            _config = config;
            _userManager = userManager;
            _tokenSetting = tokenSetting;
        }

        #region token
        public async Task<string> GenerateJwtTokenAsync(User user, bool isRemember)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDes = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = isRemember ? DateTime.Now.AddDays(1) : DateTime.Now.AddHours(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDes);

            return tokenHandler.WriteToken(token);
        }

        public async Task<GenerateTokenDto> GenerateNewToken(TokenRequestDto tokenRequestDto)
        {
            var user = await _userManager.FindByNameAsync(tokenRequestDto.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, tokenRequestDto.Password))
            {
                //create new token
                var newRefreshToken = CreateRefreshToken(_tokenSetting.ClientId, user.Id);
                //remove older tokens
                var oldRefreshToken =await _db.Tokens.Where(p => p.UserId == user.Id).ToListAsync();

                if (oldRefreshToken.Any())
                {
                    foreach (var ort in oldRefreshToken)
                    {
                        _db.Tokens.Remove(ort);
                    }
                }
                //add new refresh token to db
                _db.Tokens.Add(newRefreshToken);

                await _db.SaveChangesAsync();

                var accessToken =await GenerateJwtTokenAsync();

                return new GenerateTokenDto()
                {
                    AccessToken = accessToken,
                    Status = true
                };
            }
            else
            {
                return new GenerateTokenDto()
                {
                    Status = false
                };
            }
        }
        public Token CreateRefreshToken(string clientId,string userId)
        {
            return new Token()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                ExpireTime = DateTime.Now.AddDays(7)
            };
        }
        #endregion

        #region password

        public void CreatePasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using (var hamc = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hamc.Key;
                passwordHash = hamc.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hamc = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var cumputedHash = hamc.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < cumputedHash.Length; i++)
                {
                    if (cumputedHash[i] != passwordHash[i])
                        return false;
                }
            }
             return true;
        }

        #endregion

    }
}
