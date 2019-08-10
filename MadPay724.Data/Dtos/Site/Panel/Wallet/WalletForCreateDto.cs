using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Wallet
{
    public class WalletForCreateDto
    {
        public bool IsMain { get; set; } = false;
        public bool IsSms { get; set; } = false;

        public string Name { get; set; }

        public int Inventory { get; set; } = 0;

        public int InterMoney { get; set; } = 0;

        public int ExitMoney { get; set; } = 0;

        public int OnExitMoney { get; set; } = 0;
    }
}
