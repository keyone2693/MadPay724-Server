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

        #region tokenCreateNew

        public async Task<TokenResponseDto> GenerateNewTokenAsync(TokenRequestDto tokenRequestDto)
        {
            var user = await _userManager.FindByNameAsync(tokenRequestDto.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, tokenRequestDto.Password))
            {
                //create new token
                var newRefreshToken = CreateRefreshToken(_tokenSetting.ClientId, user.Id, tokenRequestDto.IsRemember);
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

                var accessToken =await CreateAccessTokenAsync(user, newRefreshToken.Value);

                return new TokenResponseDto()
                {
                    token = accessToken.token,
                    refresh_token = accessToken.refresh_token,
                    status = true
                };
            }
            else
            {
                return new TokenResponseDto()
                {
                    status = false,
                    message = "یوزرنیم و یا پسورد اشتباه میباشد"
                };
            }
        }

        public async Task<TokenResponseDto> CreateAccessTokenAsync(User user, string refreshToken)
        {
            double tokenExpireTime = Convert.ToDouble(_tokenSetting.ExpireTime);

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

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenSetting.Secret));
            var tokenHandler = new JwtSecurityTokenHandler();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDes = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _tokenSetting.Site,
                Audience = _tokenSetting.Audience,
                Expires =  DateTime.Now.AddMinutes(tokenExpireTime),
                SigningCredentials = creds
            };

            var newAccessToken = tokenHandler.CreateToken(tokenDes);

            var encodedAccessToken = tokenHandler.WriteToken(newAccessToken);

            return new TokenResponseDto()
            {
                token = encodedAccessToken,
                refresh_token = refreshToken
            };
        }

        public Token CreateRefreshToken(string clientId,string userId,bool isRemember)
        {
            return new Token()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                ExpireTime = isRemember ? DateTime.Now.AddDays(7) : DateTime.Now.AddDays(1)
            };
        }

        #endregion

        #region tokenRefresh

        public async Task<TokenResponseDto> RefreshAccessTokenAsync(TokenRequestDto tokenRequestDto)
        {
            try
            {
                var refreshToken =await _db.Tokens.FirstOrDefaultAsync(p =>
                    p.ClientId == _tokenSetting.ClientId && p.Value == tokenRequestDto.RefreshToken);

                if (refreshToken == null)
                {
                    return new TokenResponseDto()
                    {
                        status = false,
                        message = "خطا در اعتبار سنجی خودکار"
                    };
                }
                if(refreshToken.ExpireTime < DateTime.Now)
                {
                    return new TokenResponseDto()
                    {
                        status = false,
                        message = "خطا در اعتبار سنجی خودکار"
                    };
                }

                var user = await _userManager.FindByIdAsync(refreshToken.UserId);

                if (user == null)
                {
                    return new TokenResponseDto()
                    {
                        status = false,
                        message = "خطا در اعتبار سنجی خودکار"
                    };
                }

                var response = await CreateAccessTokenAsync(user, refreshToken.Value);

                return new TokenResponseDto()
                {
                    status = true,
                    token = response.token
                };
            }
            catch (Exception e)
            {
                return new TokenResponseDto()
                {
                    status = false,
                    message = e.Message
                };
            }
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
