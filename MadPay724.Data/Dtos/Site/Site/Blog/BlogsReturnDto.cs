using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Data.Dtos.Site.Panel.BlogGroup;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Site.Blog
{
    public class BlogsReturnDto
    {
        public List<BlogForReturnDto> Blogs { get; set; }
        public List<BlogForReturnDto> MostViewed { get; set; }
        public List<BlogForReturnDto> MostCommented { get; set; }
        public List<BlogGroupForReturnDto> BlogGroups { get; set; }
        public string LastComments { get; set; } = null;

    }

}
