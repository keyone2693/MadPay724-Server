using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Common.ReturnMessages
{
    public class ReturnMessage
    {
        public bool status { get; set; }
        public string code { get; set; }
        public string title { get; set; }
        public string message { get; set; }

    }
}
