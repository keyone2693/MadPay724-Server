using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.Users
{
    public class UserForUpdateDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool Gender { get; set; }
        public string City { get; set; }
    }
}
