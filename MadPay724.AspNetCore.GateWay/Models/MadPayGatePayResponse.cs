using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MadPay724.AspNetCore.GateWay.Models
{
  public  class MadPayGatePayResponse
    {
        [Description("توکن پرداخت")]
        public string Token { get; set; }
        [Description("آدرسی که برای پرداخت باید به آن ریدایرکت کنید")]
        public string RedirectUrl { get; set; }
    }
}
