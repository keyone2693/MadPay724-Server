using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Document;
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
    public class DocumentsControllerUnitTests
    {
        private readonly Mock<IUnitOfWork<Main_MadPayDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;

        private readonly Mock<ILogger<DocumentsController>> _mockLogger;
        private readonly Mock<IUploadService> _mockUploadService;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;

        private readonly DocumentsController _controller;

        public DocumentsControllerUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<Main_MadPayDbContext>>();
            _mockMapper = new Mock<IMapper>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockUploadService = new Mock<IUploadService>();
            _mockLogger = new Mock<ILogger<DocumentsController>>();
            _controller = new DocumentsController(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object, _mockUploadService.Object,
            _mockWebHostEnvironment.Object);

        }
        #region AddDocumentsTests
        [Fact]
        public async Task AddDocument_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.DocumentRepository.GetAsync(It.IsAny<Expression<Func<Document, bool>>>()))
                .ReturnsAsync(It.IsAny<Document>());

            _mockRepo.Setup(x => x.DocumentRepository.InsertAsync(It.IsAny<Document>()));
            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);


            _mockUploadService.Setup(x => x.UploadFileToLocal(It.IsAny<IFormFile>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.fileUploadedDto_Success);

            _mockMapper.Setup(x => x.Map(It.IsAny<DocumentForCreateDto>(), It.IsAny<Document>()))
                .Returns(UnitTestsDataInput.Users.First().Documents.First());

            _mockMapper.Setup(x => x.Map<DocumentForReturnDto>(It.IsAny<Document>()))
                .Returns(new DocumentForReturnDto());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddDocument(It.IsAny<string>(), new DocumentForCreateDto());
            var okResult = result as CreatedAtRouteResult;

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<DocumentForReturnDto>(okResult.Value);
            Assert.Equal(201, okResult.StatusCode);

        }
        [Fact]
        public async Task AddDocument_Fail_Approve_1()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.DocumentRepository.GetAsync(It.IsAny<Expression<Func<Document, bool>>>()))
                .ReturnsAsync(new Document(){Approve = 1});

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddDocument(It.IsAny<string>(), new DocumentForCreateDto());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("شما مدرک شناسایی تایید شده دارید و نمیتوانید دوباره آنرا ارسال کنید",
                okResult.Value.ToString());

            Assert.Equal(400, okResult.StatusCode);

        }
        [Fact]
        public async Task AddDocument_Fail_Approve_0()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.DocumentRepository.GetAsync(It.IsAny<Expression<Func<Document, bool>>>()))
                .ReturnsAsync(new Document() { Approve = 0});

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.AddDocument(It.IsAny<string>(), new DocumentForCreateDto());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("مدارک ارسالی قبلیه شما در حال بررسی میباشد",
                okResult.Value.ToString());

            Assert.Equal(400, okResult.StatusCode);

        }
        [Fact]
        public async Task AddDocument_Fail_WrongFile()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.DocumentRepository.GetAsync(It.IsAny<Expression<Func<Document, bool>>>()))
                .ReturnsAsync(It.IsAny<Document>());


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
            var result = await _controller.AddDocument(It.IsAny<string>(), new DocumentForCreateDto());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("فایلی برای اپلود یافت نشد",
                okResult.Value.ToString());

            Assert.Equal(400, okResult.StatusCode);

        }
        [Fact]
        public async Task AddDocument_Fail_SaveDb()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.DocumentRepository.GetAsync(It.IsAny<Expression<Func<Document, bool>>>()))
                .ReturnsAsync(It.IsAny<Document>());

            _mockRepo.Setup(x => x.DocumentRepository.InsertAsync(It.IsAny<Document>()));
            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(false);

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
            var result = await _controller.AddDocument(It.IsAny<string>(), new DocumentForCreateDto());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("خطا در ثبت اطلاعات",
                okResult.Value.ToString());

            Assert.Equal(400, okResult.StatusCode);

        }
        #endregion


        #region GetDocumentTests
        [Fact]
        public async Task GetDocument_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.DocumentRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Documents.First());

            _mockMapper.Setup(x => x.Map<DocumentForReturnDto>(It.IsAny<Document>()))
                .Returns(new DocumentForReturnDto());




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
            var result = await _controller.GetDocument(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<DocumentForReturnDto>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public async Task GetDocument_Fail_SeeAnOtherOneCard()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.DocumentRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Documents.First());


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
            var result = await _controller.GetDocument(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);

            Assert.Equal("شما اجازه دسترسی به مدرک کاربر دیگری را ندارید", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        [Fact]
        public async Task GetDocument_Fail_NullDocument()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.DocumentRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<Document>());


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetDocument(It.IsAny<string>(), It.IsAny<string>());
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal("مدرکی وجود ندارد", okResult.Value);
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion
        #region GetDocumentsTests
        [Fact]
        public async Task GetDocuments_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            //_mockRepo.Setup(x => x.DocumentRepository.GetManyAsync(
            //        It.IsAny<Expression<Func<Document, bool>>>(),
            //        It.IsAny<Func<IQueryable<Document>, IOrderedQueryable<Document>>>(),
            //        It.IsAny<string>()))
            //    .ReturnsAsync(new List<Document>());

            _mockMapper.Setup(x => x.Map<List<DocumentForReturnDto>>(It.IsAny<List<Document>>()))
                .Returns(new List<DocumentForReturnDto>());

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetDocuments(It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }


        #endregion
    }
}
