using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Users;
using MadPay724.Data.Models.MainDB;
using MadPay724.Presentation.Controllers.Site.V1.Admin;
using MadPay724.Repo.Infrastructure;
using MadPay724.Test.DataInput;
using MadPay724.Test.UnitTests.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ControllersTests
{
    public class AdminUsersControllerUnitTests
    {
        private readonly Mock<IUnitOfWork<Main_MadPayDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        //private readonly Mock<IUtilities> _mockUtilities;
        private readonly Mock<ILogger<AdminUsersController>> _mockLogger;
        private readonly AdminUsersController _controller;
        private readonly Mock<FakeUserManager> _mockUserManager;

        private readonly Mock<Main_MadPayDbContext> _dbMad;

        public AdminUsersControllerUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<Main_MadPayDbContext>>();

            _mockMapper = new Mock<IMapper>();
            //_mockUtilities = new Mock<IUtilities>();
            _mockLogger = new Mock<ILogger<AdminUsersController>>();
            _mockUserManager = new Mock<FakeUserManager>();
            _dbMad = new Mock<Main_MadPayDbContext>();
            _controller = new AdminUsersController(_mockRepo.Object, _dbMad.Object, _mockMapper.Object, _mockLogger.Object, _mockUserManager.Object);

        }

        #region GetUserTests
        [Fact]
        public async Task EditRoles_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First());

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(UnitTestsDataInput.RolesString);

            _mockUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<User>(),It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.EditRoles(It.IsAny<string>(), UnitTestsDataInput.roleEditDto);
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsAssignableFrom<IList<string>>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task EditRoles_Fail_AddToRoles()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First());

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(UnitTestsDataInput.RolesString);

            _mockUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Failed());

            string expected = "خطا در اضافه کردن نقش ها";

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.EditRoles(It.IsAny<string>(),UnitTestsDataInput.roleEditDto);
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.Equal(expected, okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }
        [Fact]
        public async Task EditRoles_Fail_RemoveFromRoles()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockUserManager.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First());

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(UnitTestsDataInput.RolesString);

            _mockUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Failed());

            string expected = "خطا در پاک کردن نقش ها";

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.EditRoles(It.IsAny<string>(), UnitTestsDataInput.roleEditDto);
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.Equal(expected, okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion
    }
}
