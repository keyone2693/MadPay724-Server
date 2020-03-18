using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models.MainDB;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Panel.User.Service;
using MadPay724.Test.DataInput;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ServicesTests
{
    public class UserServiceUnitTest
    {
        private readonly Mock<IUnitOfWork<Main_MadPayDbContext>> _mockRepo;
        private readonly Mock<IUtilities> _mockUtilities;
        private readonly UserService _service;

        public UserServiceUnitTest()
        {
            _mockRepo = new Mock<IUnitOfWork<Main_MadPayDbContext>>();
            _mockUtilities = new Mock<IUtilities>();
            _service = new UserService(_mockRepo.Object, _mockUtilities.Object);

        }

        #region GetUserForPassChangeTests
        [Fact]
        public async Task GetUserForPassChange_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First());

            _mockUtilities.Setup(x => x.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(true);


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.GetUserForPassChange(It.IsAny<string>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(result);
            Assert.IsType<User>(result);

        }
        [Fact]
        public async Task GetUserForPassChange_Fail_WrongId()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<User>());


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.GetUserForPassChange(It.IsAny<string>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Null(result);

        }
        [Fact]
        public async Task GetUserForPassChange_Fail_WrongPassword()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First());

            _mockUtilities.Setup(x => x.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
                .Returns(false);


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.GetUserForPassChange(It.IsAny<string>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.Null(result);

        }
        #endregion


        #region UpdateUserPassTests
        [Fact]
        public async Task UpdateUserPass_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            byte[] passwordHash, passwordSalt;

            _mockUtilities.Setup(x => x.CreatePasswordHash(It.IsAny<string>(), out passwordHash,out passwordSalt));

            _mockRepo.Setup(x => x.UserRepository.Update(It.IsAny<User>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);



            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.UpdateUserPass(new User(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.True(result);

        }
        [Fact]
        public async Task UpdateUserPass_Fail_dbError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            byte[] passwordHash, passwordSalt;

            _mockUtilities.Setup(x => x.CreatePasswordHash(It.IsAny<string>(), out passwordHash, out passwordSalt));

            _mockRepo.Setup(x => x.UserRepository.Update(It.IsAny<User>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(false);



            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.UpdateUserPass(new User(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.False(result);

        }
        #endregion
    }
}
