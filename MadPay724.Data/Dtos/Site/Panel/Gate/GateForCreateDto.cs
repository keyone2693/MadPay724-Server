using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Gate
{
  public  class GateForCreateDto
    {
        [Required]
        public bool IsIp { get; set; }
        [Required]
        public string WalletId { get; set; }
        [StringLength(100, MinimumLength = 0)]
        [Required]
        public string WebsiteName { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string WebsiteUrl { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string Text { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 0)]
        public string Grouping { get; set; }
        public IFormFile File { get; set; }
    }
}
