using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Models.FinancialDB.Accountant
{
    public class Entry : BaseEntity<string>
    {
        public Entry()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        public bool IsApprove { get; set; }
        [Required]
        public bool IsPardakht { get; set; }
        [Required]
        public bool IsReject { get; set; }
        [Required]
        public int Price { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string TextForUser { get; set; }
        //card
        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string BankName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 0)]
        public string OwnerName { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string Shaba { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 0)]
        public string CardNumber { get; set; }
        //wallet
        [Required]
        [StringLength(20, MinimumLength = 0)]
        public string WalletName { get; set; }
        //ids
        [Required]
        public string UserId { get; set; }
        [Required]
        public string BankCardId { get; set; }
        [Required]
        public string WalletId { get; set; }
    }
}
