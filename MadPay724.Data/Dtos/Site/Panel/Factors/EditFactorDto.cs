using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Factors
{
   public class EditFactorDto
    {
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string RefBank { get; set; }
    }
}
