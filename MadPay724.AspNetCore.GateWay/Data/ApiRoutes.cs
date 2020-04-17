using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.AspNetCore.GateWay.Data
{
    public static class ApiRoutes
    {
        public const string BaseUrl = "https://api.madpay724.ir";
        public const string BaseApi = BaseUrl + "/v1/pg";

        public static class Pay
        {
            //Post
            public const string PaySend = BaseApi + "/pay";
        }
        public static class Verify
        {
            //Post
            public const string VerifySend = BaseApi + "/verify";
        }

        public static class Refund
        {
            //Post
            public const string RefundSend = BaseApi + "/refund";
        }
    }
}
