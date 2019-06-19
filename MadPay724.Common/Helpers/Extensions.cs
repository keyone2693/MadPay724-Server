using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MadPay724.Common.Helpers
{
    public static class Extensions
    {
        public static void AddAppError(this HttpResponse response, string message)
        {
            response.Headers.Add("App-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "App-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }


        public static int ToAge(this DateTime dateTime)
        {
            var age = DateTime.Today.Year - dateTime.Year;
            if(dateTime.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        }
    }
}
