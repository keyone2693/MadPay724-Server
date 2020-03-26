using MadPay724.Data.Dtos.Site.Panel.Blog;

using System.Collections.Generic;

namespace MadPay724.Data.Dtos.Site.Site.Home
{
   public class HomeDataReturnDto
    {
        public List<CustomerDto> Customers { get; set; }
        public ServiceStatDto ServiceStat { get; set; }
        public IEnumerable<FeedBackDto> FeedBacks { get; set; }
        public IEnumerable<BlogForReturnDto> LastBlogs { get; set; }
    }
}
