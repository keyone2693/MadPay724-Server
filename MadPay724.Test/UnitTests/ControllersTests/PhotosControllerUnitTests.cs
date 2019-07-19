using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.ErrorAndMessage;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using MadPay724.Presentation.Controllers.Site.Admin;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.User.Interface;
using MadPay724.Services.Upload.Interface;
using MadPay724.Test.DataInput;
using MadPay724.Test.IntegrationTests.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ControllersTests
{
    public class PhotosControllerUnitTests
    {
        private readonly Mock<IUnitOfWork<MadpayDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUploadService> _mockUploadService;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly PhotosController _controller;

        public PhotosControllerUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<MadpayDbContext>>();
            _mockMapper = new Mock<IMapper>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockUploadService = new Mock<IUploadService>();
            _controller = new PhotosController(_mockRepo.Object, _mockMapper.Object, _mockUploadService.Object, _mockWebHostEnvironment.Object);

        }

        #region GetPhotoTests
        [Fact]
        public async Task GetPhoto_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.PhotoRepository.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Photos.First());

            _mockMapper.Setup(x => x.Map<PhotoForReturnProfileDto>(It.IsAny<Photo>()))
                .Returns(UnitTestsDataInput.PhotoForReturnProfileDto);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.GetPhoto(It.IsAny<string>());
            var okResult = result as OkObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<PhotoForReturnProfileDto>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
        }
        #endregion



        #region ChangeUserPhotoTests
        [Fact]
        public async Task ChangeUserPhoto_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockRepo.Setup(x => x.PhotoRepository.GetAsync(It.IsAny<Expression<Func<Photo, bool>>>()))
                .ReturnsAsync(UnitTestsDataInput.Users.First().Photos.First());

            _mockRepo.Setup(x => x.PhotoRepository.Update(It.IsAny<Photo>()));

            _mockRepo.Setup(x => x.SaveAsync()).ReturnsAsync(true);

            _mockUploadService.Setup(x => x.UploadProfilePic(It.IsAny<IFormFile>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.fileUploadedDto_Success);

            _mockUploadService.Setup(x => x.RemoveFileFromCloudinary(It.IsAny<string>()))
                .Returns(UnitTestsDataInput.fileUploadedDto_Success);

            _mockUploadService.Setup(x => x.RemoveFileFromLocal(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(UnitTestsDataInput.fileUploadedDto_Success);

            _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns(It.IsAny<string>());

            _mockMapper.Setup(x => x.Map(It.IsAny<PhotoForProfileDto>(), It.IsAny<Photo>()))
                .Returns(UnitTestsDataInput.Users.First().Photos.First());

            _mockMapper.Setup(x => x.Map<PhotoForReturnProfileDto>(It.IsAny<Photo>()))
                .Returns(UnitTestsDataInput.PhotoForReturnProfileDto);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };


            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.ChangeUserPhoto(It.IsAny<string>(),UnitTestsDataInput.photoForProfileDto);
            var okResult = result as CreatedAtRouteResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<PhotoForReturnProfileDto>(okResult.Value);
            Assert.Equal(201, okResult.StatusCode);
        }
        [Fact]
        public async Task ChangeUserPhoto_Fail_WorngFile()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------
            _mockUploadService.Setup(x => x.UploadProfilePic(It.IsAny<IFormFile>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(UnitTestsDataInput.fileUploadedDto_Fail_WrongFile);
            string expectedErrorMessage = "فایلی برای اپلود یافت نشد";

            _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns(It.IsAny<string>());


            var httpContext = new DefaultHttpContext();
            httpContext.Request.Scheme = "222";
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _controller.ChangeUserPhoto(It.IsAny<string>(), UnitTestsDataInput.photoForProfileDto);
            var okResult = result as BadRequestObjectResult;
            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(okResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal(expectedErrorMessage,okResult.Value.ToString());
            Assert.Equal(400, okResult.StatusCode);
        }

        #endregion
    }
}
