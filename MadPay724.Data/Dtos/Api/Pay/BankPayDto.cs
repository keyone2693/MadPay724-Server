using MadPay724.Data.Models.FinancialDB.Accountant;
using MadPay724.Data.Models.MainDB.UserModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Api.Pay
{
  public  class BankPayDto
    {
        public Factor Factor { get; set; }
        public Gate Gate { get; set; }
    }
}
