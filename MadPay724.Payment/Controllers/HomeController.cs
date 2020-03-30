using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MadPay724.Payment.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("https://madpay724.ir");
        }
    }
}