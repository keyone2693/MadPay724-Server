using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Users
{
   public class UserBlogInfoDto
    {
        public string Name { get; set; }
        public long TotalBlog { get; set; }
        public long ApprovedBlog { get; set; }
        public long UnApprovedBlog { get; set; }
    }
}
