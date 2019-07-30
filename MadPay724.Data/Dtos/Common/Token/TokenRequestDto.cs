using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Common.Token
{
   public class TokenRequestDto
    {
        [Required]
        public string GrantType { get; set; } //password || refresh_token
        [Required]
        public string ClientId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمیباشد")]
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
        public string Password { get; set; }
    }
}
