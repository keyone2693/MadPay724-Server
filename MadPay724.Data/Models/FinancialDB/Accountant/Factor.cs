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
        [StringLength(150, MinimumLength = 0)]
        public int Price { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 0)]
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
