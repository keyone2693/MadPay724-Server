using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Document
{
   public class DocumentForUpdateDto
    {
        [Required]
        public short Approve { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string Message { get; set; }
    }
}
