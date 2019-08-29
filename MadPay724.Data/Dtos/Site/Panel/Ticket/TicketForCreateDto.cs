using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Ticket
{
   public class TicketForCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string Title { get; set; }
        [Required]
        public short Level { get; set; }
        [Required]
        public short Department { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string Text { get; set; }

        public IFormFile File { get; set; }

    }
}
