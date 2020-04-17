using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.AspNetCore.GateWay.Models
{
   public class MadPayGatePayRequest
    {
        [Required(ErrorMessage = "فیلد api نمیتواند خالی باشد")]
        [StringLength(100, ErrorMessage = "فیلد api باید بین 1 تا 100 کاراکتر باشد", MinimumLength = 1)]
        [Description("API Key دریافتی از پنل کاربری شما که بعد از تایید درخواست درگاه صادر میشود")]
        public string Api { get; set; }

        [Required(ErrorMessage = "فیلد مبلغ پرداختی نمیتواند خالی باشد")]
        [Description("مبلغ تراکنش به صورت ریالی و بزرگتر یا مساوی 1000")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "فیلد آدرس برگشتی نمیتواند خالی باشد")]
        [Url(ErrorMessage = "ادرس مورد نظر برای ریدایرکت معتبر نمیباشد")]
        [Description("آدرس بازگشتی به صورت urlencode ، که باید با آدرس درگاه پرداخت تایید شده در madpay724.ir بر روی یک دامنه باشد")]
        public string RedirectUrl { get; set; }

        [StringLength(50, ErrorMessage = "فیلد نام کاربر باید کمتر از 50 کاراکتر باشد")]
        [Description("نام کاربر (اختیاری)")]
        public string UserName { get; set; } = "";

        [StringLength(20, ErrorMessage = "فیلد موبایل باید کمتر از 20 کاراکتر باشد")]
        [Description("شماره موبایل (اختیاری) (جهت نمایش کارت های خریدار به ایشان و نمایش درگاه موبایلی)")]
        public string Mobile { get; set; } = "";

        [StringLength(100, ErrorMessage = "فیلد ایمیل باید کمتر از 100 کاراکتر باشد")]
        [Description("API Key دریافتی از پنل کاربری شما که بعد از تایید درخواست درگاه صادر میشود")]
        public string Email { get; set; } = "";

        [StringLength(100, ErrorMessage = "فیلد شماره فاکتور باید کمتر از 100 کاراکتر باشد")]
        [Description("شماره فاکتور شما (اختیاری)")]
        public string FactorNumber { get; set; } = "";

        [StringLength(255, ErrorMessage = "فیلد توضیحات باید کمتر از 255 کاراکتر باشد")]
        [Description("API Key دریافتی از پنل کاربری شما که بعد از تایید درخواست درگاه صادر میشود")]
        public string Description { get; set; } = "";

        [StringLength(16, ErrorMessage = "فیلد شماره کارت باید 16 رقمی باشد")]
        [Description("توضیحات (اختیاری ، حداکثر 255 کاراکتر)")]
        public string ValidCardNumber { get; set; } = "";
    }
}
