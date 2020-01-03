using MadPay724.Data.Dtos.Site.Panel.Gate;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Data.Models.FinancialDB.Accountant;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Factors
{
   public class FactorForReturnDto
    {
        public Factor Factor { get; set; }
        public GateForReturnDto Gate { get; set; }
        public WalletForReturnDto Wallet { get; set; }
    }
}
