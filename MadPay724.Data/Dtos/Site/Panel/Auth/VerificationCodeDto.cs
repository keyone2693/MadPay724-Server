using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Auth
{
  public  class VerificationCodeDto
    {
        public string Code { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime RemoveDate { get; set; }
    }
}
