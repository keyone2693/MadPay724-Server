using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Common.Pagination
{
  public  class TicketsPaginationDto : PaginationDto
    {
        public short IsAdminSide { get; set; }
        public short Closed { get; set; }
        public short Department { get; set; }
        public short Level  { get; set; }
        public long MinDate { get; set; }
        public long MaxDate { get; set; }
    }
}

