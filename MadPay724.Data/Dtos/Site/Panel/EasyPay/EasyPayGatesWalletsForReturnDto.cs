using MadPay724.Data.Dtos.Site.Panel.Gate;
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.EasyPay
{
   public class EasyPayGatesWalletsForReturnDto
    {
        public EasyPayForReturnDto EasyPay { get; set; }
        public IEnumerable<GateForReturnDto> Gates { get; set; }
        public IEnumerable<WalletForReturnDto> Wallets { get; set; }
    }
}
