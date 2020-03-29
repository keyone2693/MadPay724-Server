using MadPay724.Common.Helpers.AppSetting;
using MadPay724.Common.Helpers.Utilities;
using MadPay724.Common.Helpers.Interface;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MadPay724.Test.UnitTests.MethodTest
{
  public  class UtilitiesTests
    {
        private readonly IUtilities _utilities;
        private readonly Mock<IConfiguration> _config;
        public UtilitiesTests()
        {
            _config = new Mock<IConfiguration>();
            _utilities = new Utilities(null, null, null, null, null);
        }
        [Fact]
        public async Task FindLocalPathFromUrl_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var str = _utilities.FindLocalPathFromUrl("www.ali.com/ali/sdf/asd/asd.png");
            var str1 = _utilities.FindLocalPathFromUrl("ali.com/ali/sdf/asd/asd.png");
            var str2 = _utilities.FindLocalPathFromUrl("https://www.ali.com/ali/sdf/asd/asd.png");
            var str3 = _utilities.FindLocalPathFromUrl("https://ali.com/ali/sdf/asd/asd.png");
            var str4 = _utilities.FindLocalPathFromUrl("http://www.ali.com/ali/sdf/asd/asd.png");
            var str5 = _utilities.FindLocalPathFromUrl("http://ali.com/ali/sdf/asd/asd.png");

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result1 = _utilities.FindLocalPathFromUrl(str1);
            var result2 = _utilities.FindLocalPathFromUrl(str2);
            var result3 = _utilities.FindLocalPathFromUrl(str3);
            var result4 = _utilities.FindLocalPathFromUrl(str4);
            var result5 = _utilities.FindLocalPathFromUrl(str5);

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.NotNull(result4);
            Assert.NotNull(result5);

            Assert.Equal("ali.com/ali/sdf/asd", result1);
            Assert.Equal("ali.com/ali/sdf/asd", result2);
            Assert.Equal("ali.com/ali/sdf/asd", result3);
            Assert.Equal("ali.com/ali/sdf/asd", result4);
            Assert.Equal("ali.com/ali/sdf/asd", result5);
        }
    }
}
