using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MadPay724.Data.Models.UserModel
{
    public class Gate : BaseEntity<string>
    {
        public Gate()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsDirect { get; set; }
        [Required]
        public bool Ip { get; set; }
        [StringLength(100, MinimumLength = 0)]
        [Required]
        public string WebsiteName { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string WebsiteUrl{ get; set; }
        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string PhonrNumber { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string Text { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string Grouping { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string IconUrl { get; set; }
        [Required]
        public string WalletId { get; set; }
        [ForeignKey("WalletId")]
        public Wallet Wallet { get; set; }
    }
}
