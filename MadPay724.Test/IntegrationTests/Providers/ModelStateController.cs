using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MadPay724.Test.IntegrationTests.Providers
{
    public class ModelStateController  : Controller
    {
        public ModelStateController()
        {
            ControllerContext = (new Mock<ControllerContext>()).Object;
        }
    }
}
