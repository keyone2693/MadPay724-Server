using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Users
{
    public class UserForRegisterDto
    {
        [Required]
        [EmailAddress(ErrorMessage ="ایمیل وارد شده صحیح نمیباشد")]
        public string UserName { get; set; }
        [Required]
        [StringLength( 10 , MinimumLength =4,ErrorMessage ="پسورد باید بین 4 رقم و ده رقم باشد")]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Phone(ErrorMessage = "شماره موبایل صحیح نمیباشد")]
        public string PhoneNumber { get; set; }
    }
}
