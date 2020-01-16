using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Common
{
   public class UserDashboardDto
    {
        public long UnClosedTicketCount { get; set; } = 0;
        public long ClosedTicketCount { get; set; } = 0;

        public IEnumerable<Models.MainDB.Ticket> Last5Tickets { get; set; }

        public long TotalInventory { get; set; } = 0;
        public DaysForReturnDto Inventory5Days { get; set; }

        public long TotalInterMoney { get; set; } = 0;
        public DaysForReturnDto InterMoney5Days { get; set; }

        public long TotalExitMoney { get; set; } = 0;
        public DaysForReturnDto ExitMoney5Days { get; set; }

        public long TotalSuccessFactor { get; set; } = 0;
        //public DaysForReturnDto Factor10Days { get; set; }
        public IEnumerable<Models.FinancialDB.Accountant.Factor> Last10Factors { get; set; }


        public long TotalSuccessEntry { get; set; } = 0;
        public IEnumerable<Models.FinancialDB.Accountant.Entry> Last10Entries { get; set; }

    }
}
