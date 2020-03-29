using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Api.Pay
{
   public class PayResponseDto
    {
        [Description("توکن پرداخت")]
        public string Token { get; set; }
        [Description("آدرسی که برای پرداخت باید به آن ریدایرکت کنید")]
        public string RedirectUrl { get; set; }
    }
}
