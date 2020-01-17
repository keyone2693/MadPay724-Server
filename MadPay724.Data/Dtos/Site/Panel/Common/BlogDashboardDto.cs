using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Data.Dtos.Site.Panel.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Common
{
   public class BlogDashboardDto
    {
        public long TotalBlogCount { get; set; } = 0;
        public long ApprovedBlogCount { get; set; } = 0;
        public long UnApprovedBlogCount { get; set; } = 0;


        public DaysForReturnDto TotalBlog5Days { get; set; }
        public DaysForReturnDto UnApprovedBlog5Days { get; set; }
        public DaysForReturnDto ApprovedBlog5Days { get; set; }


        public List<BlogForReturnDto> Last7Blogs { get; set; }

        public List<UserBlogInfoDto> Last12UserBlogInfo { get; set; }
    }
}
