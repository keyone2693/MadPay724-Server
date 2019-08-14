using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Ticket
{
   public class TicketContentForCreateDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string Text { get; set; }
        [StringLength(1000, MinimumLength = 0)]
        public string FileUrl { get; set; }
    }
}
