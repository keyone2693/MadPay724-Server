using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Models.FinancialDB.Accountant
{
   public class Factor : BaseEntity<string>
    {
        public Factor()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        [StringLength(150, MinimumLength = 0)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "فیلد url برگشتی نمیتواند خالی باشد")]
        [StringLength(1000, ErrorMessage = "فیلد url باید بین 1 تا 1000 کاراکتر باشد", MinimumLength = 1)]
        public string RedirectUrl { get; set; }

        [StringLength(20, ErrorMessage = "فیلد موبایل باید کمتر از 20 کاراکتر باشد")]
        public string Mobile { get; set; } = "";

        [StringLength(100, ErrorMessage = "فیلد ایمیل باید کمتر از 100 کاراکتر باشد")]
        public string Email { get; set; } = "";

        [StringLength(100, ErrorMessage = "فیلد شماره فاکتور باید کمتر از 100 کاراکتر باشد")]
        public string FactorNumber { get; set; } = "";

        [StringLength(255, ErrorMessage = "فیلد توضیحات باید کمتر از 255 کاراکتر باشد")]
        public string Description { get; set; } = "";

        [StringLength(16, ErrorMessage = "فیلد شماره کارت باید 16 رقمی باشد")]
        public string ValidCardNumber { get; set; } = "";


        [Required]
        public bool Status { get; set; }
        [Required]
        public short Kind { get; set; }
        [Required]
        public short Bank { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 0)]
        public string GiftCode { get; set; }
        [Required]
        public bool IsGifted { get; set; }
        

        [Required]
        public int Price { get; set; }
        [Required]
        public int EndPrice { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string RefBank { get; set; }




        [Required]
        public string EnterMoneyWalletId { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public string GateId { get; set; }

    }
}
