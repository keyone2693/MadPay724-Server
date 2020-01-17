using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Data.Dtos.Site.Panel.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Common
{
    public class AdminDashboardDto : AccountantDashboardDto
    {
        public long UnClosedTicketCount { get; set; } = 0;
        public long ClosedTicketCount { get; set; } = 0;


        public long TotalInventory { get; set; } = 0;
        public long TotalInterMoney { get; set; } = 0;
        public long TotalExitMoney { get; set; } = 0;

        public long TotalBlogCount { get; set; } = 0;
        public long ApprovedBlogCount { get; set; } = 0;
        public long UnApprovedBlogCount { get; set; } = 0;


        public DaysForReturnDto ExitMoney5Days { get; set; }
        public DaysForReturnDto InterMoney5Days { get; set; }
        public DaysForReturnDto Inventory5Days { get; set; }

        public DaysForReturnDto TotalBlog5Days { get; set; }
        public DaysForReturnDto UnApprovedBlog5Days { get; set; }
        public DaysForReturnDto ApprovedBlog5Days { get; set; }


        public List<BlogForReturnDto> Last7Blogs { get; set; }

        public List<UserBlogInfoDto> Last12UserBlogInfo { get; set; }


        public IEnumerable<Models.MainDB.Ticket> Last5Tickets { get; set; }

    }
}
