using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Ticket;
using MadPay724.Data.Models.MainDB;
using MadPay724.Presentation.Controllers.V1.Panel.User;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using MadPay724.Test.DataInput;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ControllersTests
{
   public class TicketControllerUnitTests
    {
        private readonly Mock<IUnitOfWork<Main_MadPayDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<TicketsController>> _mockLogger;
        private readonly TicketsController _controller;
        private readonly Mock<IUploadService> _mockUploadService;
        private readonly Mock<IWebHostEnvironment> _mockEenv;

        public TicketControllerUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<Main_MadPayDbContext>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<TicketsController>>();
            _mockUploadService = new Mock<IUploadService>();
            _mockEenv = new Mock<IWebHostEnvironment>();
            _controller = new TicketsController(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object,
                _mockUploadService.Object, _mockEenv.Object);
        }

        #region GetTicketTests
        [Fact]
        public async Task GetTicket_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            //_mockRepo.Setup(x => x.TicketRepository.GetManyAsync(
            //        It.IsAny<Expression<Func<Ticket, bool>>>(),
            //        It.IsAny<Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>>>(),
            //        It.IsAny<string>()))
            //    .ReturnsAsync(UnitTestsDataInput.Users.First().Tickets);

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
            var result = await _controller.GetTicket(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<Ticket>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task GetTicket_Fail_SeeAnOtherOneCard()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //_mockRepo.Setup(x => x.TicketRepository.GetManyAsync(
            //     It.IsAny<Expression<Func<Ticket, bool>>>(),
            //     It.IsAny<Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>>>(),
            //     It.IsAny<string>()))
            // .ReturnsAsync(UnitTestsDataInput.Users.First().Tickets);


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
            var result = await _controller.GetTicket(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);

            Assert.Equal("شما اجازه دسترسی به تیکت کاربر دیگری را ندارید", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task GetTicket_Fail_NullTicket()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //_mockRepo.Setup(x => x.TicketRepository.GetManyAsync(
            //    It.IsAny<Expression<Func<Ticket, bool>>>(),
            //    It.IsAny<Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>>>(),
            //    It.IsAny<string>()))
            //.ReturnsAsync(new List<Ticket>());

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetTicket(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("تیکتی وجود ندارد", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion
        #region GetTicketsTests
        [Fact]
        public async Task GetTickets_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.TicketRepository.GetManyAsyncPaging(
                    It.IsAny<Expression<Func<Ticket, bool>>>(),
                    It.IsAny<Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>>>(),
                    It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()
                    , It.IsAny<int>()))
                .ReturnsAsync(new List<Ticket>());

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetTickets(It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }


        #endregion
        #region AddTicketTests
        [Fact]
        public async Task AddTicket_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.TicketRepository.GetAsync(It.IsAny<Expression<Func<Ticket, bool>>>()))
                .ReturnsAsync(It.IsAny<Ticket>());


            _mockRepo.Setup(x => x.TicketRepository.InsertAsync(It.IsAny<Ticket>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);


            _mockMapper.Setup(x => x.Map(It.IsAny<TicketForCreateDto>(), It.IsAny<Ticket>()))
                .Returns(new Ticket());

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddTicket(It.IsAny<string>(),
                new TicketForCreateDto(){Title = "Title" });
            var okResult = result as CreatedAtRouteResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<Ticket>(okResult.Value);
            Assert.Equal(201, okResult.StatusCode);
        }
        [Fact]
        public async Task AddTicket_Fail_KarNumberRepeat()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.TicketRepository.GetAsync(It.IsAny<Expression<Func<Ticket, bool>>>()))
                .ReturnsAsync(new Ticket());
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddTicket(It.IsAny<string>(),
                new TicketForCreateDto() { Title = "Title" });
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("این تیکت قبلا ثبت شده است", okResult.Value.ToString());
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion

        //----------------------------------------------------------------------------
        #region GetTicketContentTests
        [Fact]
        public async Task GetTicketContent_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.TicketRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Tickets.First());

            _mockRepo.Setup(x => x.TicketContentRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Tickets.First().TicketContents.First());

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
            var result = await _controller.GetTicketContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<TicketContent>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task GetTicketContent_Fail_SeeAnOtherOneCard()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.TicketRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Tickets.First());

            _mockRepo.Setup(x => x.TicketContentRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Tickets.First().TicketContents.First());

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
            var result = await _controller.GetTicketContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);

            Assert.Equal("شما اجازه دسترسی به تیکت کاربر دیگری را ندارید", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task GetTicketContent_Fail_NullTicketContent()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            _mockRepo.Setup(x => x.TicketRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Tickets.First());

            _mockRepo.Setup(x => x.TicketContentRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<TicketContent>());

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
            var result = await _controller.GetTicketContent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("تیکتی وجود ندارد", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion
        #region GetTicketContentsTests
        [Fact]
        public async Task GetTicketContents_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //_mockRepo.Setup(x => x.TicketContentRepository.GetManyAsync(
            //        It.IsAny<Expression<Func<TicketContent, bool>>>(),
            //        It.IsAny<Func<IQueryable<TicketContent>, IOrderedQueryable<TicketContent>>>(),
            //        It.IsAny<string>()))
            //    .ReturnsAsync(new List<TicketContent>());

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetTicketContents(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        #endregion
        #region AddTicketContentTests
        [Fact]
        public async Task AddTicketContent_Success_WithoutFile()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            var fileMock = new Mock<IFormFile>();

            var file = fileMock.Object;

            _mockRepo.Setup(x => x.TicketContentRepository.InsertAsync(It.IsAny<TicketContent>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);


            _mockUploadService.Setup(x => x.UploadFileToLocal(It.IsAny<IFormFile>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.fileUploadedDto_Success);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddTicketContent(It.IsAny<string>(), It.IsAny<string>(),
                new TicketContentForCreateDto() { Text = "Text" ,File = file });
            var okResult = result as CreatedAtRouteResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<TicketContent>(okResult.Value);
            Assert.Equal(201, okResult.StatusCode);
        }
        [Fact]
        public async Task AddTicketContent_Success_WithFile()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;


            _mockRepo.Setup(x => x.TicketContentRepository.InsertAsync(It.IsAny<TicketContent>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);


            _mockUploadService.Setup(x => x.UploadFileToLocal(It.IsAny<IFormFile>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.fileUploadedDto_Success);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddTicketContent(It.IsAny<string>(), It.IsAny<string>(),
                new TicketContentForCreateDto() { Text = "Text" ,File = file });
            var okResult = result as CreatedAtRouteResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<TicketContent>(okResult.Value);
            Assert.Equal(201, okResult.StatusCode);
        }
        [Fact]
        public async Task AddTicketContent_Fail_UploadFail()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            _mockUploadService.Setup(x => x.UploadFileToLocal(It.IsAny<IFormFile>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.fileUploadedDto_Fail_WrongFile);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddTicketContent(It.IsAny<string>(), It.IsAny<string>(),
                new TicketContentForCreateDto() { Text = "Text",File = file});
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion
    }
}
