using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MadPay724.Data.Models
{
   public class Wallet : BaseEntity<string>
    {
        public Wallet()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }
        [Required]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Code { get; set; }
        [Required]
        public bool IsMain { get; set; }
        [Required]
        public bool IsSms { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 0)]
        public string Name { get; set; }



        [Required]
        public int Inventory { get; set; }

        [Required]
        public int InterMoney { get; set; }

        [Required]
        public int ExitMoney { get; set; }

        [Required]
        public int OnExitMoney { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
