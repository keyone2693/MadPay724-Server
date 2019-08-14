using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MadPay724.Data.Dtos.Site.Panel.Ticket
{
   public class TicketContentForCreateDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string Text { get; set; }
        public IFormFile File { get; set; }
    }
}
