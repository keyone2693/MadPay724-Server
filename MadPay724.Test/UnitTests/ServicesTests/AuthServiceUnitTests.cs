using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Models.MainDB;
using MadPay724.Presentation.Controllers.Site.V1.Panel.User;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.Auth.Service;
using MadPay724.Services.Site.Panel.User.Interface;
using MadPay724.Test.DataInput;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.Tests.ServicesUnitTests
{
    public class AuthServiceUnitTests
    {
        private readonly Mock<IUnitOfWork<Main_MadPayDbContext>> _mockRepo;
        private readonly Mock<IUtilities> _mockUtilities;
        private readonly AuthService _service;

        public AuthServiceUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<Main_MadPayDbContext>>();
            _mockUtilities = new Mock<IUtilities>();
            _service = new AuthService(_mockRepo.Object, _mockUtilities.Object);

        }

        #region LoginTests
        [Fact]
        public async Task Login_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //_mockRepo.Setup(x => x.UserRepository
            //    .GetManyAsync(
            //        It.IsAny<Expression<Func<User, bool>>>(),
            //        It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
            //        It.IsAny<string>())).ReturnsAsync(UnitTestsDataInput.Users);

            _mockUtilities.Setup(x => x.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(true);
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.LoginAsync(It.IsAny<string>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(result);
            Assert.IsType<User>(result);

        }
        [Fact]
        public async Task Login_Fail_WrongUserName()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //_mockRepo.Setup(x => x.UserRepository
            //    .GetManyAsync(
            //        It.IsAny<Expression<Func<User, bool>>>(),
            //        It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
            //        It.IsAny<string>())).ReturnsAsync(Enumerable.Empty<User>());

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.LoginAsync(It.IsAny<string>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Null(result);
        }
        [Fact]
        public async Task Login_Fail_WrongPassWord()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //_mockRepo.Setup(x => x.UserRepository
            //    .GetManyAsync(
            //        It.IsAny<Expression<Func<User, bool>>>(),
            //        It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
            //        It.IsAny<string>())).ReturnsAsync(UnitTestsDataInput.Users);

            _mockUtilities.Setup(x => x.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(false);
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.LoginAsync(It.IsAny<string>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Null(result);

        }
        #endregion

        #region RegisterTests
        [Fact]
        public async Task Register_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            byte[] passwordHash, passwordSalt;
            _mockUtilities.Setup(x => x.CreatePasswordHash(It.IsAny<string>(),
                out passwordHash, out passwordSalt));

            _mockRepo.Setup(x => x.UserRepository.InsertAsync(It.IsAny<User>()));
            _mockRepo.Setup(x => x.PhotoRepository.InsertAsync(It.IsAny<Photo>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.RegisterAsync(new User(), It.IsAny<Photo>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(result);
            Assert.IsType<User>(result);

        }
        [Fact]
        public async Task Register_Fail_dbError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            byte[] passwordHash, passwordSalt;
            _mockUtilities.Setup(x => x.CreatePasswordHash(It.IsAny<string>(),
                out passwordHash, out passwordSalt));

            _mockRepo.Setup(x => x.UserRepository.InsertAsync(It.IsAny<User>()));
            _mockRepo.Setup(x => x.PhotoRepository.InsertAsync(It.IsAny<Photo>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(false);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.RegisterAsync(new User(), It.IsAny<Photo>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Null(result);

        }
        #endregion
    }
}
