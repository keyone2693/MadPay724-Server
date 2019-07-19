using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using MadPay724.Presentation.Controllers.Site.Admin;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using MadPay724.Services.Site.Admin.User.Interface;
using MadPay724.Test.DataInput;
using MadPay724.Test.IntegrationTests.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ControllersTests
{
    public class AuthControllerUnitTests
    {
        private readonly Mock<IUnitOfWork<MadpayDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IConfigurationSection> _mockConfigSection;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<MadpayDbContext>>();
            _mockMapper = new Mock<IMapper>();
            //_mockUtilities = new Mock<IUtilities>();
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _mockConfig = new Mock<IConfiguration>();
            _mockConfigSection = new Mock<IConfigurationSection>();
            _controller = new AuthController(_mockRepo.Object, _mockAuthService.Object, _mockConfig.Object, _mockMapper.Object, _mockLogger.Object);

        }
        #region loginTests
        [Fact]
        public async Task Login_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockAuthService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(UsersControllerMockData.GetUser().First());


            _mockConfigSection.Setup(x => x.Value).Returns("Token ekn wle nwe nwer hwoe rlwken rlwke ns");
            _mockConfig.Setup(x => x.GetSection(It.IsAny<string>())).Returns(_mockConfigSection.Object);

            _mockMapper.Setup(x => x.Map<UserForDetailedDto>(It.IsAny<User>()))
                .Returns(UsersControllerMockData.GetUserForDetailedDto());
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.Login(UnitTestsDataInput.useForLoginDto_Success);
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
           // Assert.IsType<UserForDetailedDto>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task Login_Fail()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockAuthService.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<User>());
            string expected = "کاربری با این یوزر و پس وجود ندارد";

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.Login(UnitTestsDataInput.useForLoginDto_Success);
            var okResult = result as UnauthorizedObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
             Assert.IsType<string>(okResult.Value);
             Assert.Equal(expected, okResult.Value);
            Assert.Equal(401, okResult.StatusCode);
        }
        [Fact]
        public async Task Login_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var controller = new ModelStateController();
            //Act----------------------------------------------------------------------------------------------------------------------------------
            controller.ValidateModelState(UnitTestsDataInput.useForLoginDto_Fail_ModelState);
            var modelState = controller.ModelState;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.False(modelState.IsValid);
            Assert.Equal(2, modelState.Keys.Count());
            Assert.True(modelState.Keys.Contains("UserName") && modelState.Keys.Contains("Password"));
        }
        #endregion

        #region registerTests
        [Fact]
        public async Task Register_Success_UserRegister()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }
        [Fact]
        public async Task Register_Fail_UserExist()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }
        [Fact]
        public async Task Register_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }
        #endregion
    }
}
