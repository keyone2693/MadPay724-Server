using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.AspNetCore.GateWay.Models
{
   public class MadPayGateRefundRequest
    {
        [Required(ErrorMessage = "فیلد Api نمیتواند خالی باشد")]
        [StringLength(100, ErrorMessage = "فیلد Api باید بین 1 تا 100 کاراکتر باشد", MinimumLength = 1)]
        [Description("API Key دریافتی از پنل کاربری شما که بعد از تایید درخواست درگاه صادر میشود")]
        public string Api { get; set; }

        [Required(ErrorMessage = "فیلد Token نمیتواند خالی باشد")]
        [StringLength(100, ErrorMessage = "فیلد Token باید بین 1 تا 100 کاراکتر باشد", MinimumLength = 1)]
        [Description("Token ارسالی از درگاه پرداخت")]
        public string Token { get; set; }
    }
}
