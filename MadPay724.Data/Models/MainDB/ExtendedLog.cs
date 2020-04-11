using Microsoft.AspNetCore.Http;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace MadPay724.Data.Models.MainDB
{
    public class ExtendedLog : Log
    {
        public ExtendedLog(IHttpContextAccessor http)
        {
            if (http != null)
            {
                string browser = http.HttpContext.Request.Headers["User-Agent"];
                if (!string.IsNullOrEmpty(browser) && (browser.Length > 255))
                {
                    browser = browser.Substring(0, 255);
                }

                this.Browser = browser;
                this.Host = http.HttpContext.Connection?.RemoteIpAddress.ToString();
                this.User = http.HttpContext.User?.Identity.Name;
                this.Path = http.HttpContext.Request.Path;
            }
            else
            {

                this.Browser = "System Error";
                this.Host = "System Error";
                this.User = "System Error";
                this.Path = "System Error";
            }


        }
        public ExtendedLog()
        {

        }

        public string Browser { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
        public string Path { get; set; }
    }
}
