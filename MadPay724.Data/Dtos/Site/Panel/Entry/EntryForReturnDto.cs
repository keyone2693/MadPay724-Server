using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Entry
{
  public  class EntryForReturnDto
    {
        public bool IsApprove { get; set; }
        public bool IsPardakht { get; set; }
        public bool IsReject { get; set; }
        public int Price { get; set; }

        public string TextForUser { get; set; }

        public string BankName { get; set; }

        public string OwnerName { get; set; }

        public string Shaba { get; set; }

        public string CardNumber { get; set; }

        public string WalletName { get; set; }
    }
}
