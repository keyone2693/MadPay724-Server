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
using MadPay724.Data.Dtos.Site.Panel.Wallet;
using MadPay724.Data.Models;
using MadPay724.Presentation.Controllers.Site.V1.User;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Wallet.Interface;
using MadPay724.Services.Site.Admin.Wallet.Service;
using MadPay724.Test.DataInput;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ControllersTests
{
   public class WalletsControllerUnitTests
    {
        private readonly Mock<IUnitOfWork<MadpayDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<WalletsController>> _mockLogger;
        private readonly WalletsController _controller;
        private readonly Mock<IWalletService> _mockWalletService;

        public WalletsControllerUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<MadpayDbContext>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<WalletsController>>();
            _mockWalletService = new Mock<IWalletService>();
            _controller = new WalletsController(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object, _mockWalletService.Object);
        }

        #region GetWalletTests
        [Fact]
        public async Task GetWallet_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Wallets.First());

            _mockMapper.Setup(x => x.Map<WalletForReturnDto>(It.IsAny<Wallet>()))
                .Returns(new WalletForReturnDto());




            var rout = new RouteData();
            rout.Values.Add("userId", UnitTestsDataInput.userLogedInId);

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
            var result = await _controller.GetWallet(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<WalletForReturnDto>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task GetWallet_Fail_SeeAnOtherOneCard()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Wallets.First());


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
            var result = await _controller.GetWallet(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);

            Assert.Equal("شما اجازه دسترسی به کیف پول کاربر دیگری را ندارید", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task GetWallet_Fail_NullWallet()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Wallet>());


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetWallet(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("کیف پولی وجود ندارد", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion
        #region GetWalletsTests
        [Fact]
        public async Task GetWallets_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetManyAsync(
                    It.IsAny<Expression<Func<Wallet, bool>>>(),
                    It.IsAny<Func<IQueryable<Wallet>, IOrderedQueryable<Wallet>>>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new List<Wallet>());

            _mockMapper.Setup(x => x.Map<List<WalletForReturnDto>>(It.IsAny<List<Wallet>>()))
                .Returns(new List<WalletForReturnDto>());

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetWallets(It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }


        #endregion

        #region AddWalletTests
        [Fact]
        public async Task AddWallet_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetAsync(It.IsAny<Expression<Func<Wallet, bool>>>()))
                .ReturnsAsync(It.IsAny<Wallet>());

            _mockRepo.Setup(x => x.WalletRepository.WalletCountAsync(It.IsAny<string>()))
                .ReturnsAsync(5);


            _mockWalletService.Setup(x => x.CheckInventoryAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockWalletService.Setup(x => x.DecreaseInventoryAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new ReturnMessage(){status = true});

            _mockRepo.Setup(x => x.WalletRepository.InsertAsync(It.IsAny<Wallet>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);


            _mockMapper.Setup(x => x.Map<WalletForReturnDto>(It.IsAny<Wallet>()))
                .Returns(new WalletForReturnDto());

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddWallet(It.IsAny<string>(),
                UnitTestsDataInput.walletForCreateDto);
            var okResult = result as CreatedAtRouteResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<WalletForReturnDto>(okResult.Value);
            Assert.Equal(201, okResult.StatusCode);
        }
        [Fact]
        public async Task AddWallet_Fail_KarNumberRepeat()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetAsync(It.IsAny<Expression<Func<Wallet, bool>>>()))
                .ReturnsAsync(new Wallet());
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddWallet(It.IsAny<string>(),
                UnitTestsDataInput.walletForCreateDto);
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("این کیف پول قبلا ثبت شده است", okResult.Value.ToString());
            Assert.Equal(400, okResult.StatusCode);
        }
        [Fact]
        public async Task AddWallet_Fail_MoreThan10Card()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetAsync(It.IsAny<Expression<Func<Wallet, bool>>>()))
                .ReturnsAsync(It.IsAny<Wallet>());

            _mockRepo.Setup(x => x.WalletRepository.WalletCountAsync(It.IsAny<string>()))
                .ReturnsAsync(11);


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddWallet(It.IsAny<string>(),
            UnitTestsDataInput.walletForCreateDto);
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("شما اجازه وارد کردن بیش از 10 کیف پول را ندارید", okResult.Value.ToString());
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task AddWallet_Fail_NotEnoughInventory()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetAsync(It.IsAny<Expression<Func<Wallet, bool>>>()))
                .ReturnsAsync(It.IsAny<Wallet>());

            _mockRepo.Setup(x => x.WalletRepository.WalletCountAsync(It.IsAny<string>()))
                .ReturnsAsync(5);


            _mockWalletService.Setup(x => x.CheckInventoryAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(false);



            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddWallet(It.IsAny<string>(),
                UnitTestsDataInput.walletForCreateDto);
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("کیف پول انتخابی موجودی کافی ندارد", okResult.Value.ToString());
            Assert.Equal(400, okResult.StatusCode);
        }
        [Fact]
        public async Task AddWallet_Fail_InventoryDecError()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.WalletRepository.GetAsync(It.IsAny<Expression<Func<Wallet, bool>>>()))
                .ReturnsAsync(It.IsAny<Wallet>());

            _mockRepo.Setup(x => x.WalletRepository.WalletCountAsync(It.IsAny<string>()))
                .ReturnsAsync(5);


            _mockWalletService.Setup(x => x.CheckInventoryAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockWalletService.Setup(x => x.DecreaseInventoryAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new ReturnMessage() { status = false,message = ""});



            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddWallet(It.IsAny<string>(),
                UnitTestsDataInput.walletForCreateDto);
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            //Assert.Equal("خطا در کاهش موجودی", okResult.Value.ToString());
            Assert.Equal(400, okResult.StatusCode);
        }
        #endregion
    }
}
