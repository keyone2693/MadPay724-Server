using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MadPay724.Data.Models.MainDB
{
    public class Setting : BaseEntity<short>
    {
        public Setting()
        { 
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
        }

        [Required]
        public string CloudinaryCloudName { get; set; }
        [Required]
        public string CloudinaryAPIKey { get; set; }
        [Required]
        public string CloudinaryAPISecret { get; set; }

        [Required]
        public bool UploadLocal { get; set; }

    }
}
