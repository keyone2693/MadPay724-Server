using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using MadPay724.Presentation.Controllers.Site.V1.User;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Interface;
using MadPay724.Services.Site.Admin.User.Interface;
using MadPay724.Test.DataInput;
using MadPay724.Test.IntegrationTests.Providers;
using MadPay724.Test.UnitTests.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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
        private readonly Mock<IUtilities> _mockUtilities;

        private readonly Mock<FakeUserManager> _mockUserManager;
        private readonly Mock<FakeSignInManager> _mockSignInManager;

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

            _mockUserManager =new Mock<FakeUserManager>();
            _mockSignInManager = new Mock<FakeSignInManager>();


            _mockUtilities = new Mock<IUtilities>();

            _controller = new AuthController(_mockRepo.Object, _mockAuthService.Object, _mockConfig.Object, _mockMapper.Object,
                _mockLogger.Object, _mockUtilities.Object, _mockUserManager.Object, _mockSignInManager.Object);

        }
        #region loginTests
        [Fact]
        public async Task Login_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First());

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(
                    It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            _mockUtilities.Setup(x => x.GenerateJwtTokenAsync(It.IsAny<User>(), It.IsAny<bool>()))
                .ReturnsAsync(It.IsAny<string>());


            _mockMapper.Setup(x => x.Map<UserForDetailedDto>(It.IsAny<User>()))
                .Returns(UnitTestsDataInput.userForDetailedDto);


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

            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First());

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(
                    It.IsAny<User>(), It.IsAny<string>(),It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.NotAllowed);

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
        public async Task Register_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(),It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockAuthService.Setup(x => x.AddUserPhotos(It.IsAny<Photo>()))
                .ReturnsAsync(true);



            _mockMapper.Setup(x => x.Map<UserForDetailedDto>(It.IsAny<User>()))
                .Returns(UnitTestsDataInput.userForDetailedDto);
            //Act----------------------------------------------------------------------------------------------------------------------------------

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext =   httpContext
            };

            var result = await _controller.Register(UnitTestsDataInput.userForRegisterDto);
            var okResult = result as CreatedAtRouteResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<UserForDetailedDto>(okResult.Value);
            Assert.Equal(201, okResult.StatusCode);
        }
        [Fact]
        public async Task Register_Fail_UserExist()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            
            var result = await _controller.Register(UnitTestsDataInput.userForRegisterDto);
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<ReturnMessage>(okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }
        [Fact]
        public async Task Register_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var controller = new ModelStateController();
            //Act----------------------------------------------------------------------------------------------------------------------------------
            controller.ValidateModelState(UnitTestsDataInput.userForRegisterDto_Fail_ModelState);
            var modelState = controller.ModelState;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.False(modelState.IsValid);
            Assert.Equal(4, modelState.Keys.Count());
            Assert.True(modelState.Keys.Contains("UserName") && modelState.Keys.Contains("Password")
                                                             && modelState.Keys.Contains("Name") && modelState.Keys.Contains("PhoneNumber"));
        }
        #endregion
    }
}
