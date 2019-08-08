using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudinaryDotNet;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Services;
using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Site.Admin.Auth.Service;
using MadPay724.Services.Upload.Service;
using MadPay724.Test.DataInput;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace MadPay724.Test.UnitTests.ServicesTests
{
    public class UploadServiceUnitTests
    {
        private readonly Mock<IUnitOfWork<MadpayDbContext>> _mockRepo;
        private readonly Mock<IFormFile> _mockFile;

        private readonly UploadService _service;

        public UploadServiceUnitTests()
        {
            _mockRepo = new Mock<IUnitOfWork<MadpayDbContext>>();
            _mockFile = new Mock<IFormFile>();

            _mockRepo.Setup(x => x.SettingRepository.GetById(It.IsAny<short>()))
                .Returns(UnitTestsDataInput.settingForUpload);

            _service = new UploadService(_mockRepo.Object);
        }

        #region UploadProfilePicToLocalTests
        [Fact]
        public async Task UploadProfilePicToLocal_Success()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------

            var content = "asdasd";
            var fileName = "1.jpg";

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);

            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            _mockFile.Setup(x => x.OpenReadStream()).Returns(ms);
            _mockFile.Setup(x => x.FileName).Returns(fileName);
            _mockFile.Setup(x => x.Length).Returns(ms.Length);
            _mockFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None));

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.UploadFileToLocal(_mockFile.Object,
                "1", "D:\\Daneshjooyar\\ProjectFile\\MadPay724-Server\\MadPay724.Presentation\\wwwroot", "http://");

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(result);
            Assert.IsType<FileUploadedDto>(result);
            Assert.True(result.Status);

        }

        [Fact]
        public async Task UploadProfilePicToLocal_Success_WrongFile()
        {
            //Arrange------------------------------------------------------------------------------------------------------------------------------


            _mockFile.Setup(x => x.Length).Returns(0);

            //Act----------------------------------------------------------------------------------------------------------------------------------
            var result = await _service.UploadFileToLocal(_mockFile.Object,
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            //Assert-------------------------------------------------------------------------------------------------------------------------------
            Assert.NotNull(result);
            Assert.IsType<FileUploadedDto>(result);
            Assert.False(result.Status);

        }
        #endregion

    }
}
