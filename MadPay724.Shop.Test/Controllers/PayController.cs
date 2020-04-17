using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MadPay724.AspNetCore.GateWay;
using MadPay724.AspNetCore.GateWay.Models;
using Microsoft.AspNetCore.Mvc;

namespace MadPay724.Shop.Test.Controllers
{
    public class PayController : Controller
    {
        private readonly IMadPayGateWay _madPayGateWay;

        public PayController(IMadPayGateWay madPayGateWay)
        {
            _madPayGateWay = madPayGateWay;
        }
        public async Task<IActionResult> Index(int price)
        {
            var request = new MadPayGatePayRequest
            {
                Api = "5eac1a0d-47c9-433b-9b5c-847c2ba979bd",
                RedirectUrl = "https://localhost:44333/verify/index",
                FactorNumber = "968574",
                Amount = price,

                UserName = "کیوان",
                Email = "key.one72@gmail.com",
                Mobile = "09361234569",
                Description = "خرید از سایت کفش چرم",
                ValidCardNumber = ""
            };

            var result = await _madPayGateWay.PayAsync(request);
            if (result.Status)
            {
                return Redirect(result.Result.RedirectUrl);
            }
            else
            {
                return View(result);
            }
        }
    }
}