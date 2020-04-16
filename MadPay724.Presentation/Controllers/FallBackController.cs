using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace MadPay724.Presentation.Controllers
{
    [AllowAnonymous]
    public class FallBackController : Controller
    {
        public IActionResult Index()
        {
            var path = Request.Path.Value.ToString();
            if (string.IsNullOrEmpty(path) || path == "/" || path.StartsWith("app") || path.StartsWith("/app"))
            {
                return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "Clients/app", "index.html"), "text/HTML");
            }
            else if (path.StartsWith("my") || path.StartsWith("/my"))
            {
                return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "Clients/my", "index.html"), "text/HTML");
            }
            else
            {
                return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "Clients/app", "index.html"), "text/HTML");
            }
        }
    }
}