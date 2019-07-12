using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MadPay724.Test.Providers
{
    public class ModelStateControllerTests  : Controller
    {
        public ModelStateControllerTests()
        {
            ControllerContext = (new Mock<ControllerContext>()).Object;
        }
    }
}
