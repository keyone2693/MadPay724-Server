using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Admin.BankCards
{
    public class BankCardForUserDetailedDto
    {
        public string Id { get; set; }
        public string BankName { get; set; }
        public string OwnerName { get; set; }
        public string Shaba { get; set; }
        public string CardNumber { get; set; }
        public string ExpireDateMonth { get; set; }
        public string ExpireDateYear { get; set; }
    }
}
