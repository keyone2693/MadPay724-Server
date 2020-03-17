using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Common.Token
{
   public class TokenRequestDto
    {
        [Required]
        [Description("این پارامتر میتواند مقدار password و یا refresh_token را داشته باشد ")]
        public string GrantType { get; set; } //password || refresh_token
        [Description("...")]
        public string ClientId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نمیباشد")]
        [Description("نام کاربری که ایمیل میباشد")]
        public string UserName { get; set; }
        [Description("رفرش توکن مورد نیاز برای بارگزاری مجدد توکن")]
        public string RefreshToken { get; set; }
        [Description("رمز عبور کاربر")]
        public string Password { get; set; }
        [Description("در صورت true بودن به مدت 20 دقیقه فعال میباشد")]
        public bool IsRemember { get; set; } = false;
    }
}
