using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Blog
{
    public class BlogForCreateUpdateDto
    {
        [Required]
        public string BlogGroupId { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string Title { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string Tags { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 0)]
        public string SummerText { get; set; }

        public IFormFile File { get; set; }
    }
}
