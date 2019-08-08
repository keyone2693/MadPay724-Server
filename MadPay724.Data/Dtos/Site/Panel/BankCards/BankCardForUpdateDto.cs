using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.BankCards
{
    public class BankCardForUpdateDto
    {
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
        [StringLength(4, MinimumLength = 4)]
        public string ExpireDateYear { get; set; }
    }
}
