using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Common.ION
{
    public class Collection<T> : BaseDto
    {
        public T[] Value { get; set; }
    }
}
