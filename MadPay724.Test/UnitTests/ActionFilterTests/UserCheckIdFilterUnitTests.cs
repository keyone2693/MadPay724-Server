using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Presentation.Helpers.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ActionFilterTests
{
    public class UserCheckIdFilterUnitTests
    {
        private readonly Mock<IHttpContextAccessor> _mockAccessor;
        private readonly Mock<ILoggerFactory> _mockoggerFactory;
        private readonly Mock<ILogger<UserCheckIdFilter>> _mockLogger;

        public UserCheckIdFilterUnitTests()
        {
            _mockAccessor =new Mock<IHttpContextAccessor>() ;
            _mockoggerFactory = new Mock<ILoggerFactory>();
            _mockLogger = new Mock<ILogger<UserCheckIdFilter>>();
        }
        [Fact]
        public async Task UserCheckIdFilter_Success_UserIdHimself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var userId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";

            var httpContext = new DefaultHttpContext();
            var rout = new RouteData();
            rout.Values.Add("id", userId);


            var context = new ActionExecutingContext(
                new ActionContext
                {
                    HttpContext = httpContext,
                    RouteData = rout,
                    ActionDescriptor = new ActionDescriptor()
                },
                new List<IFilterMetadata>(), 
                new Dictionary<string, object>(), 
                new Mock<Controller>().Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userId),
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _mockAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            _mockAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            _mockAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            _mockoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);



            var filter = new UserCheckIdFilter(_mockoggerFactory.Object, _mockAccessor.Object);


            //Act----------------------------------------------------------------------------------------------------------------------------------
            filter.OnActionExecuting(context);
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Null(context.Result);
            //Assert.IsType<UnauthorizedResult>(context.Result);
        }
        [Fact]
        public async Task UserCheckIdFilter_Fail_UserIdAnOther()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var userId = "0d47394e-672f-4db7-898c-bfd8f32e2af7";
            var userIdAnOther = "0d47394e-672f-4db7-898c-sdfs2df";

            var httpContext = new DefaultHttpContext();
            var rout = new RouteData();
            rout.Values.Add("id", userIdAnOther);
            var context = new ActionExecutingContext(
                new ActionContext
                {
                    HttpContext = httpContext,
                    RouteData = rout,
                    ActionDescriptor = new ActionDescriptor()
                },
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userId),
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _mockAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            _mockAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            _mockAccessor.Setup(x => x.HttpContext.User).Returns(claimsPrincipal);

            _mockoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            var filter = new UserCheckIdFilter(_mockoggerFactory.Object, _mockAccessor.Object);
            //Act----------------------------------------------------------------------------------------------------------------------------------
            filter.OnActionExecuting(context);
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(context.Result);
            Assert.IsType<UnauthorizedResult>(context.Result);
        }
    }
}
