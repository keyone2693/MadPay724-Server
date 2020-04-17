using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MadPay724.AspNetCore.GateWay.Models
{
  public  class MadPayGateVerifyResponse
    {
        [Description("مبلغ پرداختی")]
        public int Amount { get; set; }
        [Description("کد رهگیری درگاه")]
        public string RefBank { get; set; }
        [Description("شماره فاکتوری ارسالی شما")]
        public string FactorNumber { get; set; }
        [Description("موبایل شما")]
        public string Mobile { get; set; }
        [Description("ایمیل شما")]
        public string Email { get; set; }
        [Description("توضیحات شما")]
        public string Description { get; set; }
        [Description("شماره کارت شما")]
        public string CardNumber { get; set; }
    }
}
