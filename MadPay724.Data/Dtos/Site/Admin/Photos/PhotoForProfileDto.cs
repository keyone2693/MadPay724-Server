using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.Photos
{
    public class PhotoForProfileDto
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string PublicId { get; set; }
    }
}
