using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.Users
{
    public class UserForUpdateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Name { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 0)]
        public string PhoneNumber { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 0)]
        public string Address { get; set; }
        [Required]
        public bool Gender { get; set; }
        [StringLength(100, MinimumLength = 0)]
        public string City { get; set; }
    }
}
