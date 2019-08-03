using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Notification;
using MadPay724.Data.Dtos.Site.Panel.Photos;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Models;
using MadPay724.Presentation.Controllers.Site.V1.User;
using MadPay724.Repo.Infrastructure;
using MadPay724.Test.DataInput;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ControllersTests
{
    public class BankCardsControllerUnitTests
    {
        private readonly Mock<IUnitOfWork<MadpayDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<BankCardsController>> _mockLogger;
        private readonly BankCardsController _controller;

        public BankCardsControllerUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<MadpayDbContext>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<BankCardsController>>();
            _controller = new BankCardsController(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        #region GetBankCardTests
        [Fact]
        public async Task GetBankCard_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.BankCardRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().BankCards.First());

            _mockMapper.Setup(x => x.Map<BankCardForReturnDto>(It.IsAny<BankCard>()))
                .Returns(new BankCardForReturnDto());


            var rout = new RouteData();
            rout.Values.Add("userId", UnitTestsDataInput.Users.First().Id);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,UnitTestsDataInput.userLogedInId),
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var mockContext = new Mock<HttpContext>();

            mockContext.SetupGet(x => x.User).Returns(claimsPrincipal);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockContext.Object,
                RouteData = rout
            };


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetBankCard(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<BankCardForReturnDto>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task GetBankCard_Fail_SeeAnOtherOnePhoto()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.BankCardRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().BankCards.First());


            var rout = new RouteData();
            rout.Values.Add("userId", UnitTestsDataInput.userLogedInId);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,UnitTestsDataInput.userAnOtherId),
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var mockContext = new Mock<HttpContext>();

            mockContext.SetupGet(x => x.User).Returns(claimsPrincipal);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockContext.Object,
                RouteData = rout
            };

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetBankCard(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);

            Assert.Equal("شما اجازه دسترسی به کارت کاربر دیگری را ندارید", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task GetBankCard_Fail_NullBankCard()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.BankCardRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<BankCard>());


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetBankCard(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("کارتی وجود ندارد", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion

    }
}
