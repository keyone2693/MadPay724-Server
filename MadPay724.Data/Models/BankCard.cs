using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MadPay724.Data.Models
{
    public class BankCard : BaseEntity<string>
    {
        public BankCard()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        public bool Approve { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string BankName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 0)]
        public string OwnerName { get; set; }

        public string Shaba { get; set; }

        public string HesabNumber { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 0)]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string ExpireDateMonth { get; set; }
        [Required]
        [StringLength(2,MinimumLength = 2)]
        public string ExpireDateYear { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
