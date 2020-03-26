using MadPay724.Data.Models.MainDB.Blog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Site.Home
{
   public class HomeDataReturnDto
    {
        public List<CustomerDto> Customers { get; set; }
        public ServiceStatDto ServiceStat { get; set; }
        public IEnumerable<FeedBackDto> FeedBacks { get; set; }
        public IEnumerable<Blog> LastBlogs { get; set; }
    }
}
