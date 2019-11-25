using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Blog
{
   public class BlogForReturnDto
    {
        public DateTime DateModified { get; set; }
        public string Id { get; set; }   
        public string Title { get; set; }
        public string Tags { get; set; }
        public string PicAddress { get; set; }
        public string Text { get; set; }
        public bool Status { get; set; }
        public string SummerText { get; set; }
        public bool IsSelected { get; set; }
        public int ViewCount { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }

        public string BlogGroupId { get; set; }
        public string BlogGroupName { get; set; }

    }
}
