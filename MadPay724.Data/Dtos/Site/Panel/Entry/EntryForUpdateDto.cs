using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Entry
{
   public class EntryForUpdateDto
    {
        [Required]
        public int Price { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string TextForUser { get; set; }
    }
}
