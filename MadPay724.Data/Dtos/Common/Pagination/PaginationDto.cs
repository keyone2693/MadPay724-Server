using MadPay724.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Common.Pagination
{
   public class PaginationDto
    {
        public int PageNumber { get; set; } = 0;

        private int pageSize = 10;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > Constants.MaxPageSize) ? Constants.MaxPageSize : value; }
        }
        public string Filter { get; set; }

        //SortHeader
        public string SortHe { get; set; }
        //SortDirection
        public string SortDir { get; set; }
    }
}
