using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Common
{
    public class AccountantDashboardDto
    {
        public long TotalSuccessEntry { get; set; } = 0;
        public long TotalSuccessEntryPrice { get; set; } = 0;


        public long TotalEntryApprove { get; set; } = 0;
        public long TotalEntryUnApprove { get; set; } = 0;

        public long TotalEntryPardakht { get; set; } = 0;
        public long TotalEntryUnPardakht { get; set; } = 0;

        public long TotalEntryReject { get; set; } = 0;
        public long TotalEntryUnReject { get; set; } = 0;


        public long TotalFactor{ get; set; } = 0;
        public long TotalFactorPrice { get; set; } = 0;

        public long TotalEasyPay { get; set; } = 0;
        public long TotalEasyPayPrice { get; set; } = 0;
        public long TotalSupport { get; set; } = 0;
        public long TotalSupportPrice { get; set; } = 0;
        public long TotalIncInventory { get; set; } = 0;
        public long TotalIncInventoryPrice { get; set; } = 0;
        public long TotalSuccessFactor { get; set; } = 0;
        public long TotalSuccessFactorPrice { get; set; } = 0;



        public DaysForReturnDto Entry5Days { get; set; }
        public DaysForReturnDto Factor5Days { get; set; }

        public DaysForReturnDto Factor12Months { get; set; }
        public DaysForReturnDto Entry12Months { get; set; }
        public DaysForReturnDto User12Months { get; set; }

        public DaysForReturnDto BankCard12Months { get; set; }
        public DaysForReturnDto Gate12Months { get; set; }
        public DaysForReturnDto Wallet12Months { get; set; }



        public IEnumerable<Models.FinancialDB.Accountant.Factor> Last7Factors { get; set; }
        public IEnumerable<Models.FinancialDB.Accountant.Entry> Last7Entries { get; set; }
    }
}
