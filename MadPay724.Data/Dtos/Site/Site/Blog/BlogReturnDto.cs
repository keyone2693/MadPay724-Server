using MadPay724.Data.Dtos.Site.Panel.Blog;
using MadPay724.Data.Dtos.Site.Panel.BlogGroup;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Site.Blog
{
    public class BlogReturnDto
    {
        public BlogForReturnDto Blog { get; set; }
        public BlogForReturnDto LeftBlog { get; set; }
        public BlogForReturnDto RightBlog { get; set; }
        public List<BlogForReturnDto> RelatedBlogs { get; set; }
        public List<BlogForReturnDto> MostViewed { get; set; }
        public List<BlogForReturnDto> MostCommented { get; set; }
        public List<BlogGroupForReturnDto> BlogGroups { get; set; }
        public string LastComments { get; set; } = null;

    }

}
