using MadPay724.AspNetCore.GateWay;
using MadPay724.AspNetCore.GateWay.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MadPay724.Shop.Test.Controllers
{
    public class VerifyController : Controller
    {
        private readonly IMadPayGateWay _madPayGateWay;

        public VerifyController(IMadPayGateWay madPayGateWay)
        {
            _madPayGateWay = madPayGateWay;
        }
        public async Task<IActionResult> Index(string token)
        {
            var model = new MadPayGateVerifyRequest
            {
                Api = "5eac1a0d-47c9-433b-9b5c-847c2ba979bd",
                Token = token
            };

            var result = await _madPayGateWay.VerifyAsync(model);

            return View(result);
        }
    }
}