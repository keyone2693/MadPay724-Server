using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Models;

namespace MadPay724.Data.Dtos.Common.Token
{
   public class TokenResponseDto
    {
        public string token { get; set; }
        public string refresh_token { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public User user { get; set; }

    }
}
