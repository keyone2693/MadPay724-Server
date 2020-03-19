using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Users
{
    public class UserForRegisterDto
    {
        [Required]
        [Phone(ErrorMessage = "شماره موبایل صحیح نمیباشد")]
        public string UserName { get; set; }
        [Required]
        [StringLength( 10 , MinimumLength =4,ErrorMessage ="پسورد باید بین 4 رقم و ده رقم باشد")]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "کد فعالسازی باید 5 رقمی باشد")]

        public string Code { get; set; }
    }
}
