using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Common.Pagination
{
   public class FactorPaginationDto: PaginationDto
    {
        public int Status { get; set; }
        public int FactorType { get; set; }
        public int Bank { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public long MinDate { get; set; }
        public long MaxDate { get; set; }
    }
}
