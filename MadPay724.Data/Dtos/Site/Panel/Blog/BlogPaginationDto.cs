using MadPay724.Data.Dtos.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Blog
{
    public  class BlogPaginationDto : PaginationDto
    {
        
        public string Id { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
        public string Tags { get; set; }
        public string PicAddress { get; set; }
        public string Text { get; set; }
        public string SummerText { get; set; }
        public string BlogGroupName { get; set; }
    }
}
