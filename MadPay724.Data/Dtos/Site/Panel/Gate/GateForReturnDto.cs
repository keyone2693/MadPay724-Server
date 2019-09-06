using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Gate
{
    public class GateForReturnDto
    {
        public string Id { get; set; }
        public string WalletId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDirect { get; set; }
        public bool IsIp { get; set; }
        public string WebsiteName { get; set; }
        public string WebsiteUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Text { get; set; }
        public string Grouping { get; set; }
        public string IconUrl { get; set; }
    }
}
