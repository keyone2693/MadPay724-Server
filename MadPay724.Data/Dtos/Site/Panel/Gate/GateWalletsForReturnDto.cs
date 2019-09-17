using MadPay724.Data.Dtos.Site.Panel.Wallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Gate
{
   public class GateWalletsForReturnDto
    {
        public GateForReturnDto Gate { get; set; }
        public IEnumerable<WalletForReturnDto> Wallets { get; set; }
    }
}
