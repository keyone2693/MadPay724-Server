using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using MadPay724.Presentation.Controllers.Site.Admin;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.User.Interface;
using MadPay724.Test.UnitTests.Mock.Data;
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
            _mockLogger = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_mockRepo.Object, _mockMapper.Object, _mockUserService.Object, _mockLogger.Object);
            //0d47394e-672f-4db7-898c-bfd8f32e2af7
            //haysmathis@barkarama.com
            //123789
        }

        #region GetUserTests
        [Fact]
        public async Task GetUser_Success_GetUserHimself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var users = UsersControllerData.GetUser();
            var userForDetailedDto = UsersControllerData.GetUserForDetailedDto();
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
        [Fact]
        public async Task GetUser_Fail_GetAnOtherUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }

        #endregion

        #region UpdateUserTests
        [Fact]
        public async Task UpdateUser_Success_UpdateUserHimself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }
        [Fact]
        public async Task UpdateUser_Fail_UpdateAnOtherUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

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
        public async Task ChangeUserPassword_Success_Himself()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }
        [Fact]
        public async Task ChangeUserPassword_Fail_AnOtherUser()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            //Act----------------------------------------------------------------------------------------------------------------------------------

            //Assert-------------------------------------------------------------------------------------------------------------------------------

        }
        [Fact]
        public async Task ChangeUserPassword_Fail_Himself_WrongOldPassword()
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
