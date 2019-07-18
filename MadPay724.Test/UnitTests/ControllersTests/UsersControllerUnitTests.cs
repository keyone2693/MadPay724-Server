using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ReturnMessages;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using MadPay724.Presentation.Controllers.Site.Admin;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.User.Interface;
using MadPay724.Test.DataInput;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ControllersTests
{
    public class UsersControllerUnitTests
    {
        private readonly Mock<IUnitOfWork<MadpayDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UsersController>> _mockLogger;
        private readonly UsersController _controller;

        public UsersControllerUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<MadpayDbContext>>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger =new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_mockRepo.Object, _mockMapper.Object, _mockUserService.Object, _mockLogger.Object);

        }

        #region GetUserTests
        [Fact]
        public async Task GetUser_Success_GetUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var users = UsersControllerMockData.GetUser();
            var userForDetailedDto = UsersControllerMockData.GetUserForDetailedDto();
            _mockRepo.Setup(x => x.UserRepository
                .GetManyAsync(
                    It.IsAny<Expression<Func<User, bool>>>(),
                    It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                    It.IsAny<string>())).ReturnsAsync(() => users);
            //
            _mockMapper.Setup(x => x.Map<UserForDetailedDto>(It.IsAny<User>()))
                .Returns(userForDetailedDto);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetUser(It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<UserForDetailedDto>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }

        #endregion

        #region UpdateUserTests
        [Fact]
        public async Task UpdateUser_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var users = UsersControllerMockData.GetUser();
            //var userForDetailedDto = UsersControllerMockData.GetUserForDetailedDto();
            _mockRepo.Setup(x => x.UserRepository
                .GetByIdAsync(
                    It.IsAny<string>())).ReturnsAsync(() => users.First());

            _mockRepo.Setup(x => x.UserRepository
                .Update(
                    It.IsAny<User>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);
            //
            _mockMapper.Setup(x => x.Map(It.IsAny<UserForUpdateDto>(), It.IsAny<User>()))
                .Returns(users.First());

            //Act----------------------------------------------------------------------------------------------------------------------------------

            var result = await _controller.UpdateUser(It.IsAny<string>(), It.IsAny<UserForUpdateDto>());
            var okResult = result as NoContentResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.Equal(204, okResult.StatusCode);
        }
        [Fact]
        public async Task UpdateUser_Fail()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var users = UsersControllerMockData.GetUser();
            //var userForDetailedDto = UsersControllerMockData.GetUserForDetailedDto();
            _mockRepo.Setup(x => x.UserRepository
                .GetByIdAsync(
                    It.IsAny<string>())).ReturnsAsync(() => users.First());

            _mockRepo.Setup(x => x.UserRepository
                .Update(
                    It.IsAny<User>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(false);
            //
            _mockMapper.Setup(x => x.Map(It.IsAny<UserForUpdateDto>(), It.IsAny<User>()))
                .Returns(users.First());
            //

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.UpdateUser(It.IsAny<string>(), UnitTestsDataInput.userForUpdateDto_Fail);
            var badResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(badResult);
            Assert.IsType<ReturnMessage>(badResult.Value);
            Assert.Equal(400, badResult.StatusCode);
        }
        [Fact]
        public async Task UpdateUser_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------


        }
        #endregion

        #region ChangeUserPasswordTests
        [Fact]
        public async Task ChangeUserPassword_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }
        [Fact]
        public async Task ChangeUserPassword_Fail_WrongOldPassword()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }
        [Fact]
        public async Task ChangeUserPassword_Fail_ModelStateError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------


        }
        #endregion
    }
}
