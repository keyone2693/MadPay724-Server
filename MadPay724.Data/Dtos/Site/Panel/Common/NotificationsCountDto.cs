using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Common
{
   public class NotificationsCountDto
    {
        public long UnVerifiedBlogCount { get; set; } = 0;
        public long UnClosedTicketCount { get; set; } = 0;

        public long UnCheckedEntry { get; set; } = 0;
        public long UnSpecifiedEntry { get; set; } = 0;

        public long UnVerifiedGateInPast7Days { get; set; } = 0;
        public long UnVerifiedBankCardInPast7Days { get; set; } = 0;

        public long UnVerifiedDocuments { get; set; } = 0;

    }
}
