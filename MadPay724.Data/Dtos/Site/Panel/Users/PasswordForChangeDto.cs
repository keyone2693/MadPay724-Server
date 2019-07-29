using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Users
{
    public class PasswordForChangeDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "پسورد باید بین 4 رقم و ده رقم باشد")]
        public string NewPassword { get; set; }
    }
}
